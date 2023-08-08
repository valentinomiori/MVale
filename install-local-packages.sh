#!/bin/bash

cd "$(dirname "${BASH_SOURCE[0]}")"

#Run the pack routines for a project.
Pack()
{
    # Create the package.
    dotnet pack -c Debug --include-source

    # For every package add it to the source.
    for i in bin/Debug/*.nupkg; do
        [ -f "$i" ] || break

        dotnet nuget push "$i" -s MVale
    done
}

dotnet nuget locals all -c

cd source/MVale.Core
Pack
cd ../../

cd source/MVale.Core.Utils
Pack
cd ../../

cd source/MVale.EF6
Pack
cd ../../

cd source/MVale.EFCore3
Pack
cd ../../

cd source/MVale.EFCore6
Pack
cd ../../

cd source/MVale.EFCore7
Pack
cd ../../

#cd source/MVale.EntityFramework
#Pack
#cd ../../
