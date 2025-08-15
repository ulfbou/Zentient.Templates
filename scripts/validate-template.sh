#!/bin/bash
# Zentient Templates - Single Template Validation Script (validate-template.sh)
# This script performs both static metadata and functional package validation for a single template.

# Exit immediately if a command exits with a non-zero status.
set -e

# --- Configuration and Environment Setup ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
TEMP_DIR="/tmp/template-validation"
LOG_FILE=""
TEMPLATE_DIR=""

LOG_FILE=""
TEMPLATE_DIR=""

mkdir -p "$TEMP_DIR"

# --- Color and Logging Functions ---
red() { echo -e "\033[31m$1\033[0m"; }
green() { echo -e "\033[32m$1\033[0m"; }
yellow() { echo -e "\033[33m$1\033[0m"; }
blue() { echo -e "\033[34m$1\033[0m"; }
cyan() { echo -e "\033[36m$1\033[0m"; }
bold() { echo -e "\033[1m$1\033[0m"; }

# Check if LOG_FILE is set before using tee
log() {
    local msg="$(date '+%H:%M:%S') $1"
    if [[ -n "$LOG_FILE" ]]; then
        echo "$msg" | tee -a "$LOG_FILE"
    else
        echo "$msg"
    fi
}
step() { log "$(cyan "🔄 $1")"; }
success() { log "$(green "✅ $1")"; }
error() { log "$(red "❌ $1")"; }
warning() { log "$(yellow "⚠️  $1")"; }
info() { log "$(blue "ℹ️  $1")"; }

# --- Test Results Tracking ---
TESTS_TOTAL=0
TESTS_PASSED=0
TESTS_FAILED=0
FAILED_TESTS=()

test_result() {
    TESTS_TOTAL=$((TESTS_TOTAL + 1))
    if [[ $1 -eq 0 ]]; then
        TESTS_PASSED=$((TESTS_PASSED + 1))
        success "$2"
    else
        TESTS_FAILED=$((TESTS_FAILED + 1))
        FAILED_TESTS+=("$2")
        error "$2"
    fi
}

# --- Argument Parsing and Usage ---
usage() {
    echo "Usage: $0 --template-dir <template-directory-path>"
    echo "Example: $0 --template-dir templates/zentient-library-template"
    exit 1
}

# Parse command line arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --template-dir) TEMPLATE_DIR="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

if [[ -z "$TEMPLATE_DIR" ]]; then
    usage
fi

# Set a log file path based on the template directory name
TEMPLATE_SHORT_NAME=$(basename "$TEMPLATE_DIR")
LOG_FILE="$TEMP_DIR/$TEMPLATE_SHORT_NAME/metadata-validation-$(date +%Y%m%d-%H%M%S).log"

# --- Core Validation Logic ---
setup_environment() {
    mkdir -p "$TEMP_DIR/$TEMPLATE_SHORT_NAME"
    
    step "Setting up test environment for template '$TEMPLATE_DIR'..."
    TEST_DIR="$TEMP_DIR/$TEMPLATE_SHORT_NAME"
    
    rm -rf "$TEST_DIR"
    mkdir -p "$TEST_DIR"
    cd "$TEST_DIR"

    success "Test environment ready at '$TEST_DIR'"
    info "Log file: $LOG_FILE"
}

validate_metadata() {
    step "Starting static metadata validation for '$TEMPLATE_DIR'..."
    local template_json_path="$REPO_ROOT/$TEMPLATE_DIR/.template.config/template.json"

    # Check if the template directory and json file exists
    if [[ ! -d "$REPO_ROOT/$TEMPLATE_DIR" ]]; then
        test_result 1 "Template directory not found"
        return 1
    fi
    test_result 0 "Template directory found"
    
    if [[ ! -f "$template_json_path" ]]; then
        test_result 1 "template.json not found"
        return 1
    fi
    test_result 0 "template.json found"

    # Use jq to read and validate metadata fields
    info "Validating required fields in template.json..."
    
    local shortName=$(cat "$template_json_path" | jq -r '.shortName')
    if [[ -z "$shortName" || "$shortName" == "null" ]]; then
        test_result 1 "Required field 'shortName' is missing or empty."
    else
        test_result 0 "shortName: '$shortName' is valid."
    fi

    local name=$(cat "$template_json_path" | jq -r '.name')
    if [[ -z "$name" || "$name" == "null" ]]; then
        test_result 1 "Required field 'name' is missing or empty."
    else
        test_result 0 "name: '$name' is valid."
    fi
    
    local author=$(cat "$template_json_path" | jq -r '.author')
    if [[ -z "$author" || "$author" == "null" ]]; then
        test_result 1 "Required field 'author' is missing or empty."
    else
        test_result 0 "author: '$author' is valid."
    fi

    local identity=$(cat "$template_json_path" | jq -r '.identity')
    if [[ -z "$identity" || "$identity" == "null" ]]; then
        test_result 1 "Required field 'identity' is missing or empty."
    else
        test_result 0 "identity: '$identity' is valid."
    fi
    
    local sourceName=$(cat "$template_json_path" | jq -r '.sourceName')
    if [[ -z "$sourceName" || "$sourceName" == "null" ]]; then
        test_result 1 "Required field 'sourceName' is missing or empty."
    else
        test_result 0 "sourceName: '$sourceName' is valid."
    fi
}

validate_functional_metadata() {
    local template_json_path="$REPO_ROOT/$TEMPLATE_DIR/.template.config/template.json"
    local TEMPLATE_SHORT_NAME=$(cat "$template_json_path" | jq -r '.shortName')
    local project_path="$TEST_DIR/TestProject"
    
    step "Starting functional metadata validation for '$TEMPLATE_SHORT_NAME'"

    # Install the template
    cd "$REPO_ROOT"
    info "Installing template '$TEMPLATE_SHORT_NAME' from: $TEMPLATE_DIR"
    dotnet new install "$TEMPLATE_DIR" --force >> "$LOG_FILE" 2>&1
    cd "$TEST_DIR"

    # Create project from template with metadata arguments
    info "Creating project from template '$TEMPLATE_SHORT_NAME'..."
    if ! dotnet new "$TEMPLATE_SHORT_NAME" \
        --name "TestProject" \
        --Author "Test Author" \
        --Company "Test Company" \
        --Description "Test project description for validation" \
        --RepositoryUrl "https://github.com/test/test-project" \
        --output "$project_path" \
        --force >> "$LOG_FILE" 2>&1; then
        test_result 1 "Template creation failed"
        return 1
    fi
    test_result 0 "Template creation"

    cd "$project_path"

    # --- FIX: Patch incorrect project names in the solution file ---
    step "Patching solution file for correct project paths..."
    # The template generates projects with the name 'Zentient.LibraryTemplate', not 'Zentient.NewLibrary'.
    local old_name_src="Zentient.LibraryTemplate.csproj"
    local old_name_test="Zentient.LibraryTemplate.Tests.csproj"
    local new_name_src="TestProject.csproj"
    local new_name_test="TestProject.Tests.csproj"
    
    # We use sed to replace the incorrect project paths
    sed -i "s/$old_name_src/$new_name_src/g" "TestProject.sln"
    sed -i "s/$old_name_test/$new_name_test/g" "TestProject.sln"

    # Restore packages to ensure project is runnable
    info "Restoring NuGet packages..."
    
    local restore_log="$TEST_DIR/restore-output.log"
    if ! dotnet restore > "$restore_log" 2>&1; then
        test_result 1 "Project restore failed"
        cat "$restore_log"
        return 1
    fi

    # Check for errors in the log file
    if grep -qE "error|fail" "$restore_log"; then
        test_result 1 "Project restore failed (errors found in log)"
        cat "$restore_log"
        return 1
    fi

    test_result 0 "Project restore"

    # --- Final FIX: Use dotnet publish and allow it to build the project ---
    # `dotnet publish` will now perform a full build and validation, which is what is needed.
    info "Validating project with 'dotnet publish'..."
    
    local publish_log="$TEST_DIR/publish-output.log"
    # Remove the --no-build flag to allow the command to build the project first.
    if ! dotnet publish "src/TestProject.csproj" -c Release > "$publish_log" 2>&1; then
        test_result 1 "Project validation failed (dotnet publish exited with error)"
        cat "$publish_log"
        return 1
    fi
    
    # Check for validation errors in the log file
    if grep -q "error" "$publish_log"; then
        test_result 1 "Project validation failed (errors found in log)"
        cat "$publish_log"
        return 1
    fi
    
    # Check for template placeholders in the log file
    if grep -q "LIBRARY_\|PROJECT_\|REPOSITORY_URL" "$publish_log"; then
        test_result 1 "Template placeholders found in published metadata"
        grep "LIBRARY_\|PROJECT_\|REPOSITORY_URL" "$publish_log"
        return 1
    fi

    test_result 0 "Project validation with publish"
    success "Template '$TEMPLATE_SHORT_NAME' validation successful"
}
# The 'generate_report' function is the same as before.
generate_report() {
    step "Generating test report..."
    local report_file="$TEST_DIR/metadata-report.md"
    local success_rate=0
    if [[ $TESTS_TOTAL -gt 0 ]]; then
        success_rate=$(echo "scale=1; $TESTS_PASSED * 100 / $TESTS_TOTAL" | bc 2>/dev/null || echo "0")
    fi

    echo "---" > "$report_file"
    echo "# Zentient Template Metadata Report - \`$TEMPLATE_SHORT_NAME\`" >> "$report_file"
    echo "" >> "$report_file"
    echo "**Date**: $(date)" >> "$report_file"
    echo "**Logs**: \`$LOG_FILE\`" >> "$report_file"
    echo "" >> "$report_file"
    echo "## Summary" >> "$report_file"
    echo "" >> "$report_file"
    echo "- **Total Tests**: $TESTS_TOTAL" >> "$report_file"
    echo "- **Passed**: $TESTS_PASSED" >> "$report_file"
    echo "- **Failed**: $TESTS_FAILED" >> "$report_file"
    echo "- **Success Rate**: ${success_rate}%" >> "$report_file"
    echo "" >> "$report_file"
    if [[ $TESTS_FAILED -gt 0 ]]; then
        echo "## Failed Tests" >> "$report_file"
        echo "" >> "$report_file"
        for failed_test in "${FAILED_TESTS[@]}"; do
            echo "- ❌ $failed_test" >> "$report_file"
        done
    else
        echo "## Status: ✅ All Tests Passed!" >> "$report_file"
    fi
    echo "---" >> "$report_file"

    log "Report generated: $report_file"
    cd "$REPO_ROOT"
}

# The main execution block is the same as before.
main() {
    setup_environment
    validate_metadata
    validate_functional_metadata
    generate_report
    
    # Clean up
    step "Cleaning up temporary files..."
    rm -rf "$TEMP_DIR/$TEMPLATE_SHORT_NAME"
    
    echo ""
    echo "$(cyan '==================================================================')"
    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo "$(bold "$(green "✅ ALL METADATA TESTS PASSED for '$TEMPLATE_DIR'")")"
        exit 0
    else
        echo "$(bold "$(red "❌ METADATA TESTS FAILED for '$TEMPLATE_DIR'")")"
        exit 1
    fi
}

main
