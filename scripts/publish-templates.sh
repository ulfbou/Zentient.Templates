#!/bin/bash

# Publish Zentient templates to NuGet (publish-template.sh)
set -e

# Get the version from the environment variable
VERSION="$VERSION"

if [ -z "$VERSION" ]; then
  echo "❌ ERROR: Version not provided. Please set the VERSION environment variable."
  exit 1
fi

echo "🚀 Starting NuGet publish process for version: $VERSION"

# Define your NuGet source URL
NUGET_SOURCE="https://api.nuget.org/v3/index.json"

# Find all template directories
TEMPLATE_DIRS=$(find templates -maxdepth 1 -mindepth 1 -type d)

for TEMPLATE_DIR in $TEMPLATE_DIRS; do
  TEMPLATE_ID=$(basename "$TEMPLATE_DIR")

  echo "📦 Processing template: $TEMPLATE_ID"

  # Pack the template with the correct SemVer version
  # We use the -p:PackageVersion parameter to explicitly set the version number
  dotnet pack "$TEMPLATE_DIR" -c Release -o ./artifacts/packages /p:PackageVersion=$VERSION --no-restore --force
  
  # The NuGet package name is typically the directory name. The version number is also explicit.
  NUPKG_FILE="./artifacts/packages/$TEMPLATE_ID.$VERSION.nupkg"

  if [ -f "$NUPKG_FILE" ]; then
    echo "Publishing $NUPKG_FILE..."
    # Push the package using the API key
    dotnet nuget push "$NUPKG_FILE" \
      --source "$NUGET_SOURCE" \
      --api-key "$NUGET_API_KEY" \
      --skip-duplicate
    echo "✅ Published successfully."
  else
    echo "❌ No nupkg file found for template '$TEMPLATE_ID' with version '$VERSION' at path: $NUPKG_FILE"
    exit 1
  fi
done

echo "✅ All templates published successfully."