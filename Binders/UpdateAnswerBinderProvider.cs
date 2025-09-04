using Microsoft.AspNetCore.Mvc.ModelBinding;
using Skii.DTOs;

namespace Skii.Binders;

public class UpdateAnswerBinderProvider:IModelBinderProvider

    
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(UpdateAnswerParsedDto) ? new UpdateAnswerBinder() : null;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromUpdateAnswerAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Custom;
    }
