namespace SpatialAnalyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UpdateCodeFix))]
    [Shared]
    public class UpdateCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("CS0618");

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var syntaxRoot = await document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);
            var model = await document.GetSemanticModelAsync();

            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (string.IsNullOrEmpty(token.ValueText))
                {
                    continue;
                }

                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
                if (node is ObjectCreationExpressionSyntax objectCreation &&
                    objectCreation.Type is SimpleNameSyntax simpleName)
                {
                    if (simpleName.Identifier.ValueText == "UnitVector3D")
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Use Create",
                                _ => Task.FromResult(document.WithSyntaxRoot(
                                    syntaxRoot.ReplaceNode(
                                        objectCreation,
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.ParseExpression("UnitVector3D.Create"),
                                            objectCreation.ArgumentList))))),
                            diagnostic);
                    }
                    else if (simpleName.Identifier.ValueText == "Circle3D")
                    {
                        var args = objectCreation.ArgumentList;
                        if (args.Arguments.Count == 3)
                        {
                            var arg3 = args.Arguments[2];
                            if (model.GetTypeInfo(arg3.ChildNodes().First()).Type.Name == "Point3D")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use Create",
                                        _ => Task.FromResult(document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                objectCreation,
                                                SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.ParseExpression("Circle3D.FromPoints"),
                                                    objectCreation.ArgumentList))))),
                                    diagnostic);
                            }
                            else if (model.GetTypeInfo(arg3.ChildNodes().First()).Type.Name == "UnitVector3D")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use Create",
                                        _ => Task.FromResult(document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                objectCreation,
                                                SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.ParseExpression("Circle3D.FromPointsAndAxis"),
                                                    objectCreation.ArgumentList))))),
                                    diagnostic);
                             }
                        }
                    }
                    else if (simpleName.Identifier.ValueText == "Angle" &&
                             objectCreation.ArgumentList != null &&
                             objectCreation.ArgumentList.Arguments.Count == 2 &&
                             objectCreation.ArgumentList.Arguments[1] is ArgumentSyntax argument &&
                             argument.Expression is MemberAccessExpressionSyntax memberAccess &&
                             memberAccess.Name is SimpleNameSyntax unitName)
                    {
                        if (unitName.Identifier.ValueText == "Degrees")
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Use Create",
                                    _ => Task.FromResult(
                                        document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                objectCreation,
                                                AngleFromDegrees(objectCreation.ArgumentList.Arguments[0]))))),
                                diagnostic);
                        }
                        else if (unitName.Identifier.ValueText == "Radians")
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Use Create",
                                    _ => Task.FromResult(document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            objectCreation,
                                            AngleFromRadians(objectCreation.ArgumentList.Arguments[0]))))),
                                diagnostic);
                        }
                    }
                }
                else if (node is InvocationExpressionSyntax invocation &&
                         invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is IdentifierNameSyntax identifierName &&
                        identifierName.Identifier.ValueText == "Angle" &&
                        memberAccess.Name.Identifier.ValueText == "From" &&
                        invocation.ArgumentList != null &&
                        invocation.ArgumentList.Arguments.Count == 2 &&
                        invocation.ArgumentList.Arguments[1] is ArgumentSyntax angleArgument)
                    {
                        if (angleArgument.Expression is MemberAccessExpressionSyntax argMemberAccess &&
                            argMemberAccess.Name is SimpleNameSyntax unitName)
                        {
                            if (unitName.Identifier.ValueText == "Degrees")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use FromDegrees",
                                        _ => Task.FromResult(document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                invocation,
                                                AngleFromDegrees(invocation.ArgumentList.Arguments[0]))))),
                                    diagnostic);
                            }
                            else if (unitName.Identifier.ValueText == "Radians")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use FromRadians",
                                        _ => Task.FromResult(document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                invocation,
                                                AngleFromRadians(invocation.ArgumentList.Arguments[0]))))),
                                    diagnostic);
                            }
                        }
                    }
                    else if (memberAccess.Name.Identifier.ValueText == "Rotate" &&
                             invocation.ArgumentList != null &&
                             invocation.ArgumentList.Arguments.Count == 3 &&
                             invocation.ArgumentList.Arguments[2] is ArgumentSyntax angleArg)
                    {
                        if (angleArg.Expression is MemberAccessExpressionSyntax argMemberAccess &&
                            argMemberAccess.Name is SimpleNameSyntax unitName)
                        {
                            if (unitName.Identifier.ValueText == "Degrees")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use Angle.FromDegrees",
                                        _ => Task.FromResult(
                                            document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    invocation.ArgumentList,
                                                    invocation.ArgumentList.WithArguments(
                                                        invocation.ArgumentList.Arguments
                                                                  .Replace(
                                                                      angleArg,
                                                                      angleArg.WithExpression(AngleFromDegrees(invocation.ArgumentList.Arguments[1])))
                                                                  .RemoveAt(1)))))),
                                    diagnostic);
                            }
                            else if (unitName.Identifier.ValueText == "Radians")
                            {
                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Use Angle.FromRadians",
                                        _ => Task.FromResult(
                                            document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    invocation.ArgumentList,
                                                    invocation.ArgumentList.WithArguments(
                                                        invocation.ArgumentList.Arguments
                                                                  .Replace(
                                                                      angleArg,
                                                                      angleArg.WithExpression(AngleFromRadians(invocation.ArgumentList.Arguments[1])))
                                                                  .RemoveAt(1)))))),
                                    diagnostic);
                            }
                        }
                    }
                }
                else if (node is BinaryExpressionSyntax binaryExpression &&
                         binaryExpression.IsKind(SyntaxKind.MultiplyExpression))
                {
                    var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
                    if (message.StartsWith("'Degrees.operator *(double, Degrees)' is obsolete"))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Use Create",
                                _ => Task.FromResult(
                                    document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            binaryExpression,
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.ParseExpression("Angle.FromDegrees"),
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(binaryExpression.Left)))))))),
                            diagnostic);
                    }
                    else if (message.StartsWith("'Radians.operator *(double, Radians)' is obsolete"))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Use Create",
                                _ => Task.FromResult(
                                    document.WithSyntaxRoot(
                                        syntaxRoot.ReplaceNode(
                                            binaryExpression,
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.ParseExpression("Angle.FromRadians"),
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(binaryExpression.Left)))))))),
                            diagnostic);
                    }
                }
            }
        }

        private static InvocationExpressionSyntax AngleFromDegrees(ArgumentSyntax valueArgument)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.ParseExpression("Angle.FromDegrees"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(valueArgument)));
        }

        private static InvocationExpressionSyntax AngleFromRadians(ArgumentSyntax valueArgument)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.ParseExpression("Angle.FromRadians"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(valueArgument)));
        }
    }
}
