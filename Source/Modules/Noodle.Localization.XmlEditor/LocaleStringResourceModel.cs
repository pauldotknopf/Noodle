namespace Noodle.Localization.XmlEditor
{
    public class LocaleStringResourceModel
    {
        private readonly LocaleStringResource _resource;

        public LocaleStringResourceModel(LocaleStringResource resource)
        {
            IsMissing = false;
            _resource = resource;
            if (_resource == null)
            {
                _resource = new LocaleStringResource();
                IsMissing = true;
            }
        }

        public LocaleStringResourceModel()
            : this(null)
        {
            
        }

        public string ResourceName
        {
            get { return _resource.ResourceName; }
            set { _resource.ResourceName = value; }
        }

        public string ResourceValue
        {
            get { return _resource.ResourceValue; }
            set { _resource.ResourceValue = value; }
        }

        public bool IsMissing { get; set; }
    }
}
