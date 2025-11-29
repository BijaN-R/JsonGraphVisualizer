namespace JsonGraphVisualizer.Models
{
    public enum NodeType
    {
        Root,           // نود ریشه (نمایش داده نمی‌شود)
        PropertyGroup,  // گروه property های ساده
        Object,         // یک object
        Array,           // یک array
        Primitive
    } 
}
