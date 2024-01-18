# CsLox

This project is a C# implementation of the Lox scripting language as described in Robert Nystrom's book [Crafting Interpreters](http://craftinginterpreters.com/). It is an excellent guide to creating an interpeter and you should definitely check it out!

## Building

This is a .NET 6 project and can be built using `dotnet build`. Some code is auto-generated using a Python script; thus the AST definitions can be updated if required by running `python codegen.py`.

## Usage

An interactive command-line environment can be started by running `dotnet run`, alternatively a script file (such as the included examples) can be executed e.g. `dotnet run Examples/loops.lox`.