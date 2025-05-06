#!/bin/bash
# post_process_sln.sh
#
# Usage:
# ./post_process_sln.sh <solution_file> <includePresentation> <includeTests>
#
# Parameters:
# solution_file Path to the .sln file to be post-processed.
# includePresentation "true" or "false". If not "true", the Presentation block will be removed.
# includeTests "true" or "false". If not "true", the Tests block will be removed.

# Exit immediately if a command fails.
set -e

# Verify that exactly three arguments are provided.
if [ "$#" -ne 3 ]; then
echo "Usage: $0 <solution_file> <includePresentation> <includeTests>"
exit 1
fi

SOLUTION_FILE="$1"
INCLUDE_PRESENTATION="$2"
INCLUDE_TESTS="$3"

# Check if the solution file exists.
if [ ! -f "$SOLUTION_FILE" ]; then
echo "Error: Solution file '$SOLUTION_FILE' not found." >&2
exit 1
fi

# Function to remove a project block based on a keyword.
remove_project_block() {
local keyword="$1"
# The sed command uses the address range feature:
# From a line starting with "Project(" that contains the keyword,
# through the next line starting with "EndProject", delete all lines.
sed -i.bak -E "/^Project\\(\".*${keyword}.*$/,/^EndProject$/d" "$SOLUTION_FILE"
}

# Remove Presentation block if not included.
if [ "$INCLUDE_PRESENTATION" != "true" ]; then
echo "Removing Presentation project block..."
remove_project_block "Presentation"
fi

# Remove Tests block if not included.
if [ "$INCLUDE_TESTS" != "true" ]; then
echo "Removing Tests project block..."
remove_project_block "Tests"
fi

# (Optional) Tidy up excess blank lines (three or more consecutive newlines are replaced by two newlines).
sed -i.bak -E ':a;N;$!ba;s/(\n){3,}/\n\n/g' "$SOLUTION_FILE"

echo "Post-processing of the solution file completed successfully."
