# FavoritesSorter

A program to interactively sort your favorite things through pairwise comparisons

## How it works

Enter your favorite TV shows, movies, books (etc) in the text box, one per line, then click **Sort**. The program asks you which of the two you prefer, building a sorted list. Two algorithms are available:

- **Linear merge sort** — compares adjacent elements in each partition as it merges.
- **Binary merge sort** — uses binary search to find where each element belongs, reducing the number of comparisons in some cases.

## Features

- Resizable custom dialog for pairwise comparisons (with labeled buttons).
- Memory cache avoids asking the same comparison twice.
