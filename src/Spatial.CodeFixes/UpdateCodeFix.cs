namespace SpatialAnalyzers
{
    using System.Collections.Immutable;
    using System.Composition;
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

            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (string.IsNullOrEmpty(token.ValueText))
                {
                    continue;
                }

                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
                if (node is ObjectCreationExpressionSyntax objectCreation &&
                    objectCreation.Type is SimpleNameSyntax simpleName &&
                    simpleName.Identifier.ValueText == "UnitVector3D")
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
            }
        }
    }
}
