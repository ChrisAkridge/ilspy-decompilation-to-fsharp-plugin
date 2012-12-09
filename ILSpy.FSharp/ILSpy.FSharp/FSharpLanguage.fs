﻿namespace ILSpy.FSharp

open System
open System.Windows
open System.ComponentModel.Composition
open System.Linq
open System.Windows.Controls
open ICSharpCode.Decompiler
open ICSharpCode.Decompiler.Ast
open ICSharpCode.ILSpy
open ICSharpCode.NRefactory.CSharp
open Mono.Cecil

[<Class>]
[<Export(typeof<Language>)>]
type FSharpLanguage() =
    
    inherit Language()
    
    override this.Name
        with get() = "F#"
    
    override this.FileExtension
        with get() = ".fs"

    override this.ProjectFileExtension
        with get() = ".fsproj"
    
    override this.DecompileMethod(methodDefinition:MethodDefinition, output: ITextOutput, options: DecompilationOptions) = 
        output.WriteLine "This is method"
        if methodDefinition.Body <> null then
            output.WriteLine ("Size of method: " + methodDefinition.Body.CodeSize.ToString() + " bytes")
            
            let smartOutput = output :?> ISmartTextOutput

            if smartOutput <> null
            then
                smartOutput.AddButton(null, "Click me!", new RoutedEventHandler(fun sender e -> (sender :?> Button).Content <- "I was clicked!"))
                smartOutput.WriteLine()
            
            let c = new DecompilerContext(methodDefinition.Module)
            c.Settings <- options.DecompilerSettings
            c.CurrentType <- methodDefinition.DeclaringType
            let b = new AstBuilder(c)
            b.AddMethod methodDefinition
            b.RunTransformations()
            output.WriteLine("Decompiled AST has " + b.SyntaxTree.DescendantsAndSelf.Count().ToString() + " nodes")
            
            output.WriteLine("Children " + b.SyntaxTree.Children.Count().ToString())
            
    override this.DecompileProperty(property: PropertyDefinition, output: ITextOutput, options: DecompilationOptions) =
        output.WriteLine "This is property"
            
        let c = new DecompilerContext(property.Module)
        c.Settings <- options.DecompilerSettings
        c.CurrentType <- property.DeclaringType
        let b = new AstBuilder(c)
        b.AddProperty property
        b.RunTransformations()
        output.WriteLine("Decompiled AST has " + b.SyntaxTree.DescendantsAndSelf.Count().ToString() + " nodes")
            
        output.WriteLine("NodeType: " + b.SyntaxTree.NodeType.ToString())


    override this.DecompileField(field: FieldDefinition, output: ITextOutput, options: DecompilationOptions) =
        output.WriteLine "This is field"

    override this.DecompileEvent(ev: EventDefinition, output: ITextOutput, options: DecompilationOptions) =
        output.WriteLine "This is event"

    override this.DecompileType(typeDef: TypeDefinition, output: ITextOutput, options: DecompilationOptions) =
        //output.WriteLine "This is type"

        let c = new DecompilerContext(typeDef.Module)
        c.Settings <- options.DecompilerSettings
        c.CurrentType <- typeDef.DeclaringType
        let b = new AstBuilder(c)
        b.AddType typeDef
        b.RunTransformations()
        //output.WriteLine("Decompiled AST has " + b.SyntaxTree.DescendantsAndSelf.Count().ToString() + " nodes")

        let printer = new CSharpASTPrinter()
        //printer.PrintWhatIsThere(b.SyntaxTree, output)
        printer.PrintAST(b.SyntaxTree, output)


    override this.DecompileAssembly(assembly: LoadedAssembly, output: ITextOutput, options: DecompilationOptions) =
        output.WriteLine "This is assembly"