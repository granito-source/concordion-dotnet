namespace Concordion.Api.Extension;

[AttributeUsage(AttributeTargets.Class)]
public class ExtensionsAttribute : Attribute
{
    public Type[] ExtensionTypes { get; set; }

    public ExtensionsAttribute(params Type[] extensionTypes)
    {
        ExtensionTypes = extensionTypes;
    }
}
