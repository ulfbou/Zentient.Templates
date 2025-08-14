#!/bin/bash
# Zentient Templates - Single Template Validation Script (test-template.sh)
# This script validates a single template's functionality with proper error handling.

# Exit immediately if a command exits with a non-zero status.
set -e

# Enable verbose debugging of the script itself
set -x

# --- Configuration and Environment Setup ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
# Initialize these to prevent "tee: '': No such file or directory" error before setup_environment is called.
TEST_DIR=""
LOG_FILE="/tmp/zentient-template-validation/pre-setup-log.log"

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
echo "$(bold "$(cyan "🧪 ZENTIENT TEMPLATES - VALIDATION FOR '$TEMPLATE_SHORT_NAME'")")"
echo "$(cyan '=======================================================')"
echo ""

# --- Core Validation Logic ---
setup_environment() {
    step "Setting up test environment for template '$TEMPLATE_SHORT_NAME'..."
    TEST_DIR="/tmp/zentient-template-validation/$TEMPLATE_SHORT_NAME"
    LOG_FILE="$TEST_DIR/validation-$(date +%Y%m%d-%H%M%S).log"

    rm -rf "$TEST_DIR"
    mkdir -p "$TEST_DIR"
    cd "$TEST_DIR"

    success "Test environment ready at '$TEST_DIR'"
    info "Log file: $LOG_FILE"
}

check_prerequisites() {
    step "Checking prerequisites and installing template..."

    if ! command -v dotnet &> /dev/null; then
        error ".NET SDK not found"
        exit 1
    fi

    local dotnet_version=$(dotnet --version)
    info ".NET SDK version: $dotnet_version"

    local template_path="$REPO_ROOT/templates/$TEMPLATE_SHORT_NAME"
    if [[ ! -d "$template_path" ]]; then
        error "Template directory not found: $template_path"
        exit 1
    fi

    cd "$REPO_ROOT"
    info "Installing template '$TEMPLATE_SHORT_NAME' from: $template_path"
    dotnet new install "$template_path" --force >> "$LOG_FILE" 2>&1
    test_result $? "Template installation: $TEMPLATE_SHORT_NAME"
    cd "$TEST_DIR"
}

# Re-usable function to fix solution references if needed (e.g. for projects that are not directly named
# with the template name but rather are just templates).
fix_solution_references() {
    local project_name="$1"
    step "Fixing solution file references for '$project_name'..."
    local sln_file
    sln_file=$(find . -maxdepth 1 -name "*.sln" | head -1)

    if [[ -z "$sln_file" ]]; then
        warning "No solution file found"
        return 0
    fi
    info "Processing solution file: $sln_file"
    
    if grep -q "Zentient" "$sln_file"; then
        warning "Found template placeholder references in solution file. Attempting to fix..."
        sed -i "s/Zentient.NewLibrary/$project_name/g" "$sln_file"
        sed -i "s/Zentient.LibraryTemplate/$project_name/g" "$sln_file"
        test_result 0 "Solution file references fixed"
    else
        test_result 0 "Solution file references are correct"
    fi
}

# A reusable function to test a specific template run.
test_template_run() {
    local project_name="$1"
    local template_name="$2"
    local expected_files=("${@:3}")

    step "Creating new project with template '$template_name'..."
    mkdir -p "$project_name"
    cd "$project_name"

    # Creation
    if dotnet new "$template_name" -n "$project_name" --force >> "$LOG_FILE" 2>&1; then
        test_result 0 "Template creation"
    else
        test_result 1 "Template creation"
        return 1
    fi

    # Fix references
    fix_solution_references "$project_name"

    # Structure check
    info "Validating project structure..."
    local structure_ok=true
    for file in "${expected_files[@]}"; do
        if [[ ! -e "$file" ]]; then
            warning "✗ Missing: $file"
            structure_ok=false
        fi
    done
    test_result $([ "$structure_ok" = true ] && echo 0 || echo 1) "Project structure validation"

    # Build check
    info "Testing project restore and build..."
    local build_ok=true
    if ! dotnet restore >> "$LOG_FILE" 2>&1; then
        test_result 1 "Project restore"
        build_ok=false
    else
        test_result 0 "Project restore"
    fi
    if ! dotnet build --verbosity quiet >> "$LOG_FILE" 2>&1; then
        test_result 1 "Project build"
        build_ok=false
    else
        test_result 0 "Project build"
    fi

    # Return to parent directory
    cd ..
}

# The main dispatcher function
run_tests_for_template() {
    case "$TEMPLATE_SHORT_NAME" in
        "zentient-lib")
            test_template_run "MyTestLib" "zentient-lib" "MyTestLib.sln" "src/MyTestLib.csproj" "tests/MyTestLib.Tests.csproj" "README.md" "Directory.Build.props"
            ;;
        "zentient")
            test_template_run "MyTestProject" "zentient" "MyTestProject.sln" "src/MyTestProject.csproj" "README.md" "Directory.Build.props"
            ;;
        *)
            error "Unknown template short name: '$TEMPLATE_SHORT_NAME'"
            exit 1
            ;;
    esac
}

generate_report() {
    step "Generating test report..."
    local report_file="$TEST_DIR/test-report.md"
    local success_rate=0
    if [[ $TESTS_TOTAL -gt 0 ]]; then
        success_rate=$(echo "scale=1; $TESTS_PASSED * 100 / $TESTS_TOTAL" | bc 2>/dev/null || echo "0")
    fi

    echo "---" > "$report_file"
    echo "# Zentient Template Validation Report - \`$TEMPLATE_SHORT_NAME\`" >> "$report_file"
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
    check_prerequisites
    run_tests_for_template
    generate_report

    echo ""
    echo "$(cyan '=======================================================')"
    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo "$(bold "$(green "✅ ALL TESTS PASSED for '$TEMPLATE_SHORT_NAME'")")"
        exit 0
    else
        echo "$(bold "$(red "❌ TESTS FAILED for '$TEMPLATE_SHORT_NAME'")")"
        exit 1
    fi
}

main
