# FavoritesSorter

A program to interactively sort your favorite things through pairwise comparisons

[![C#](https://img.shields.io/badge/language-C%23-68217A.svg?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET Framework](https://img.shields.io/badge/Framework-4.8-512BD4.svg?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/platform/support/policy/dotnet-framework)
[![License: GPL-2.0-only](https://img.shields.io/badge/License-GPL--2.0--only-2D6CDF.svg)](LICENSE)
[![Windows](https://img.shields.io/badge/Windows-0078D6.svg?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyMyAyMyI+PHBhdGggZmlsbD0iI2YzZjNmMyIgZD0iTTAgMGgyM3YyM0gweiIvPjxwYXRoIGZpbGw9IiNmMzUzMjUiIGQ9Ik0xIDFoMTB2MTBIMXoiLz48cGF0aCBmaWxsPSIjODFiYzA2IiBkPSJNMTIgMWgxMHYxMEgxMnoiLz48cGF0aCBmaWxsPSIjMDVhNmYwIiBkPSJNMSAxMmgxMHYxMEgxeiIvPjxwYXRoIGZpbGw9IiNmZmJhMDgiIGQ9Ik0xMiAxMmgxMHYxMEgxMnoiLz48L3N2Zz4=)](https://www.microsoft.com/windows)

## How it works

Enter your favorite TV shows, movies, books (etc) in the text box, one per line, then click **Sort**. The program asks you which of the two you prefer, building a sorted list. Two algorithms are available:

- **Linear merge sort** — compares adjacent elements in each partition as it merges.
- **Binary merge sort** — uses binary search to find where each element belongs, reducing the number of comparisons in some cases.

## Features

- Resizable custom dialog for pairwise comparisons (with labeled buttons).
- Memory cache avoids asking the same comparison twice.

## AI Policy

Contributions from AI agents are welcome so long as they're reviewed by humans before committing — all changes MUST be approved by a real person, not merely accepted by an automated process or another agent.