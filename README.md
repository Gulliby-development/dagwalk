<p align="center"><img src="docs/logo.svg" width="96" alt="" /></p>

# dagwalk

[![CI](https://github.com/Gulliby-development/dagwalk/actions/workflows/ci.yml/badge.svg)](https://github.com/Gulliby-development/dagwalk/actions/workflows/ci.yml) [![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

dagwalk is a small, standard-library-only command-line tool for planning dependency work. It reads a dependency file from standard input and can produce a valid build or migration order, group independent work into levels, explain a shortest dependency path, list cycles, and report file problems. It is intended for people maintaining build scripts, CI pipelines, migration sets, and other collections of dependent steps.

The application targets .NET 10 and supports Windows, macOS, and Linux when built with the .NET 10 SDK. The POSIX shell examples below are examples for POSIX shells; the executable itself is cross-platform.

## Build and run

From the repository root, build the solution:

```sh
dotnet build
```

Run the command-line application from the resulting assembly, for example:

```sh
dotnet src/Dagwalk/bin/Debug/net10.0/dagwalk.dll order < steps.txt
```

All five commands read UTF-8 dependency text from standard input. Successful results go to standard output. Ordinary errors produce no standard output and write `dagwalk: MESSAGE` followed by a line feed to standard error. Command-line errors exit with status 2; invalid input or unknown names exit with status 1. The `check` command is the exception: it prints its problem report to standard output and exits 1 when problems are found.

dagwalk reads standard input only when the first argument is one of the five command names, even if later arguments make that command invalid. Help, version, missing-command, and unknown-command invocations do not consume standard input.

## Commands

```text
usage: dagwalk order [--from NAME] [--tie input|name]
       dagwalk levels [--from NAME]
       dagwalk why FROM TO
       dagwalk cycles
       dagwalk check
```

- `order` prints every selected node once, with dependencies before their dependents. By default, among currently available nodes it chooses first appearance order. `--tie name` chooses ordinal name order instead (for example, uppercase letters sort before lowercase letters). `--from NAME` restricts output to NAME and its transitive dependencies.
- `levels` uses the same selection as `order`, but groups nodes by dependency height. Nodes without dependencies are level 0; each other node is one greater than its highest dependency. Output levels ascend, and names within each level are in appearance order.
- `why FROM TO` first checks that FROM is a node, then checks TO; if either is unknown, it reports that name. It prints a shortest chain following dependency edges from FROM to TO. If FROM and TO are the same node, it prints just that node's name. When multiple shortest chains exist, it chooses the chain whose first differing node has the earliest appearance. A chain may traverse cycles but never repeats a node.
- `cycles` prints each cyclic strongly connected group: nodes in a group can all reach one another. Groups and their members are in appearance order. A single node is reported when it depends on itself.
- `check` reports undeclared dependencies first, in target declaration and dependency-written order, then the cycle groups. Repeated dependency edges are reported once. A clean file prints `ok`.

With no arguments, dagwalk prints the usage text to standard error and exits 2. The first argument `help`, `-h`, or `--help` prints usage to standard output and exits 0; `--version` prints `dagwalk 0.1.0` and exits 0. These special first arguments take precedence over any following words.

Options are read left to right. Only `order` accepts `--from` and `--tie`; `levels` accepts `--from`; the other commands accept neither. Each option consumes the next word as its value, even if it looks like an option. Unknown options and options not supported by the selected command are errors. An option without a following word reports a missing value before a repeated-option check; otherwise, repeating an option reports an error. `--tie` accepts exactly `input` or `name`, and defaults to `input`. Words not beginning with two hyphens are positional arguments; `why` requires exactly FROM and TO, and all other commands take none.

## Dependency file

Input is UTF-8 text split into lines at LF. One CR immediately before an LF is removed. Spaces and tabs around a line are ignored. Empty lines and lines whose first non-space/tab character is `#` are comments and skipped, but still count toward diagnostic line numbers.

Each other line has this form:

```text
NAME: DEP DEP ...
```

The first colon separates the target from its dependencies. Spaces and tabs around the target are ignored; dependencies are separated by one or more spaces or tabs, and there may be none. A name must contain 1–24 characters, each an ASCII letter, digit, underscore, period, or hyphen. A colon is not allowed inside a name. A final line need not end in LF, and an empty file is valid.

Every target and dependency names a graph node. Nodes receive appearance numbers when first encountered: a line's target is encountered before that line's dependencies, and lines are read top to bottom. A dependency repeated on one target line contributes only one edge. Nodes mentioned only as dependencies are implicit nodes with no dependencies of their own. These appearance numbers determine appearance order; name tie-breaking uses ordinal ordering, not locale-specific ordering.

The first malformed line stops parsing. Line errors are checked in this order: missing colon, invalid target or dependency name (target first, then dependencies left to right), and duplicate target declaration.

## Results and diagnostics

An empty dependency file makes `order`, `levels`, and `cycles` print nothing and exit 0. A syntactically valid `why FROM TO` on an empty file reports the first unknown name and exits 1. An empty file passed to `check` prints `ok` and exits 0. An empty selection from `order` or `levels` also prints nothing.

The following error messages are written to standard error with the `dagwalk: ` prefix and a trailing line feed; the status is 2 for command-line errors and 1 for file/name/path errors:

- `unknown command WORD`
- `unknown option WORD`
- `option WORD is not used by COMMAND`
- `missing value for WORD`
- `option given twice: WORD`
- `bad value for --tie: VALUE`
- `why needs FROM and TO`
- `COMMAND takes no arguments`
- `line N: missing colon`
- `line N: bad name`
- `line N: duplicate target NAME`
- `unknown target NAME`
- `cycle blocks: NAMES` when selected nodes cannot be ordered; NAMES includes every selected node on a cycle or dependent on one, in ordinal order
- `no dependency path from FROM to TO`

A `check` report is written to standard output. Each undeclared edge appears as `unknown dependency: NAME -> DEP`; each cyclic group appears as `cycle: NAMES`. It exits 1 if either kind of problem is present, and 0 otherwise.

## Examples

In the examples, `dagwalk` stands for `dotnet src/Dagwalk/bin/Debug/net10.0/dagwalk.dll`. The following transcripts are for a POSIX shell in the repository root after `dotnet build`. Define the shell function first:

```sh
dagwalk() { dotnet src/Dagwalk/bin/Debug/net10.0/dagwalk.dll "$@"; }
```

```text
$ printf 'app: lib util\nlib: util core\nutil:\ncore: util\n' | dagwalk order
util
core
lib
app
$ printf 'app: lib util\nlib: util core\nutil:\ncore: util\n' | dagwalk levels
L0: util
L1: core
L2: lib
L3: app
$ printf 'app: lib util\nlib: util core\nutil:\ncore: util\n' | dagwalk why app core
app -> lib -> core
$ printf 'a: b\nb: c\nc: a\nd: e\n' | dagwalk check; echo "exit $?"
unknown dependency: d -> e
cycle: a b c
exit 1
```

## License

dagwalk is distributed under the MIT License. See [LICENSE](LICENSE) for the license terms.

## Development

Run the build, formatting check, and automated test suite from the repository root:

```sh
dotnet build
dotnet format --verify-no-changes
dotnet test
```

---

Built with [BAKED](https://github.com/Gulliby-development/baked), an open-source coding agent (bench round R91).
