namespace LuisActions.Samples.Web
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Cognitive.LUIS.ActionBinding;

    public class LuisActionModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext context)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request;
            var type = request.Form["LuisActionType"]; 
            if (!string.IsNullOrWhiteSpace(type))
            {
                var action = Activator.CreateInstance(Type.GetType(type));

                Func<object> modelAccessor = () => action;
                context.ModelMetadata = new ModelMetadata(
                    new DataAnnotationsModelMetadataProvider(),
                    context.ModelMetadata.ContainerType,
                    modelAccessor,
                    action.GetType(),
                    context.ModelName);

                return base.BindModel(controllerContext, context);
            }

            return null;
        }
    }

    public class LuisActionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            if (modelType == typeof(ILuisAction))
            {
                return new LuisActionModelBinder();
            }

            return null;
        }
    }
}