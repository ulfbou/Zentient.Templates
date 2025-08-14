#!/bin/bash
# Zentient Templates - Single Template Metadata Validation Script (validate-template-metadata.sh)
# This script validates that a single template generates projects with proper NuGet metadata.

# Exit immediately if a command exits with a non-zero status.
set -e

# --- Configuration and Environment Setup ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
TEMP_DIR="/tmp/template-validation"

# --- Color and Logging Functions ---
red() { echo -e "\033[31m$1\033[0m"; }
green() { echo -e "\033[32m$1\033[0m"; }
yellow() { echo -e "\033[33m$1\033[0m"; }
blue() { echo -e "\033[34m$1\033[0m"; }
cyan() { echo -e "\033[36m$1\033[0m"; }
bold() { echo -e "\033[1m$1\033[0m"; }

log() {
    local msg="$(date '+%H:%M:%S') $1"
    echo "$msg" | tee -a "$LOG_FILE"
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
    echo "Usage: $0 --template <template-short-name>"
    echo "Example: $0 --template zentient-lib"
    exit 1
}

# Parse command line arguments
TEMPLATE_SHORT_NAME=""
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --template) TEMPLATE_SHORT_NAME="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

if [[ -z "$TEMPLATE_SHORT_NAME" ]]; then
    usage
fi

echo ""
echo "$(bold "$(cyan "🧪 ZENTIENT TEMPLATES - METADATA VALIDATION FOR '$TEMPLATE_SHORT_NAME'")")"
echo "$(cyan '==================================================================')"
echo ""

# --- Core Validation Logic ---
setup_environment() {
    step "Setting up test environment for template '$TEMPLATE_SHORT_NAME'..."
    TEST_DIR="$TEMP_DIR/$TEMPLATE_SHORT_NAME"
    LOG_FILE="$TEST_DIR/metadata-validation-$(date +%Y%m%d-%H%M%S).log"

    rm -rf "$TEST_DIR"
    mkdir -p "$TEST_DIR"
    cd "$TEST_DIR"

    success "Test environment ready at '$TEST_DIR'"
    info "Log file: $LOG_FILE"
}

validate_template() {
    local template_name="$1"
    local project_path="$TEST_DIR/TestProject"
    
    step "Validating template: '$template_name'"

    # Create project from template with metadata arguments
    info "Creating project from template '$template_name'..."
    if ! dotnet new "$template_name" \
        --name "TestProject" \
        --Author "Test Author" \
        --Company "Test Company" \
        --Description "Test project description for validation" \
        --RepositoryUrl "https://github.com/test/test-project" \
        --Tags "test;validation;template" \
        --output "$project_path" \
        --force >> "$LOG_FILE" 2>&1; then
        test_result 1 "Template creation failed"
        return 1
    fi
    test_result 0 "Template creation"

    cd "$project_path"

    # Restore packages to ensure project is runnable
    info "Restoring NuGet packages..."
    if ! dotnet restore --verbosity quiet >> "$LOG_FILE" 2>&1; then
        test_result 1 "Project restore failed"
        return 1
    fi
    test_result 0 "Project restore"

    # Validate package can be created with a dry run
    info "Validating NuGet package creation..."
    local pack_log="$TEST_DIR/pack-output.log"
    if ! dotnet pack --no-build --dry-run --verbosity normal > "$pack_log" 2>&1; then
        test_result 1 "Package validation failed (dotnet pack exited with error)"
        cat "$pack_log"
        return 1
    fi

    # Check for validation errors in the log file
    if grep -q "error" "$pack_log"; then
        test_result 1 "Package validation failed (errors found in log)"
        cat "$pack_log"
        return 1
    fi
    
    # Check for template placeholders in the log file
    if grep -q "LIBRARY_\|PROJECT_\|REPOSITORY_URL" "$pack_log"; then
        test_result 1 "Template placeholders found in package metadata"
        grep "LIBRARY_\|PROJECT_\|REPOSITORY_URL" "$pack_log"
        return 1
    fi

    test_result 0 "NuGet metadata validation"
    success "Template '$template_name' validation successful"

    cd "$TEMP_DIR" > /dev/null
}

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

# --- Main Execution ---
main() {
    setup_environment
    validate_template "$TEMPLATE_SHORT_NAME"
    generate_report
    
    # Clean up
    step "Cleaning up temporary files..."
    rm -rf "$TEMP_DIR/$TEMPLATE_SHORT_NAME"
    
    echo ""
    echo "$(cyan '==================================================================')"
    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo "$(bold "$(green "✅ ALL METADATA TESTS PASSED for '$TEMPLATE_SHORT_NAME'")")"
        exit 0
    else
        echo "$(bold "$(red "❌ METADATA TESTS FAILED for '$TEMPLATE_SHORT_NAME'")")"
        exit 1
    fi
}

main
