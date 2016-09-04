namespace MonoFrame.ContentManager
{
    /// <summary>
    /// Content Resource generic class for defining a content object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContentResource<T>
    {
        public T Content { get; private set; }
        public string ResourceString { get; set; }

        public ContentResource(T inContent, string inResourceString)
        {
            Content = inContent;
            ResourceString = inResourceString;
        }
    }
}
