using Microsoft.AspNetCore.Mvc.ModelBinding;
using Skii.Models;

namespace Skii.Binders;
//todo ???
public class CreateAnswerBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(Answer) ? new CreateAnswerBinder() : null;
    }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FromCreateAnswerAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Custom;
}