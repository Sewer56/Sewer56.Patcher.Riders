#!/bin/bash

# Check if pwsh is installed
if ! command -v pwsh &> /dev/null; then
    echo "PowerShell Core (pwsh) is not installed."
    echo "Please install from your package manager."
    echo "If unsure, follow: https://aka.ms/powershell"
    echo ""
    read -p "Press any key to continue..."
    exit 1
fi

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Execute the PowerShell script
pwsh -ExecutionPolicy Bypass -File "$SCRIPT_DIR/MAKE_PATCH.ps1"
