using GalaSoft.MvvmLight;

namespace Noodle.Localization.XmlEditor.ViewModel
{
    /// <summary>
    /// Represents a resource value
    /// </summary>
    public class LocaleStringResourceModel : ViewModelBase
    {
        private readonly LocaleStringResource _resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleStringResourceModel"/> class.
        /// Initializing this way indicates that the resource exists
        /// </summary>
        /// <param name="resource">The existing resoruce.</param>
        public LocaleStringResourceModel(LocaleStringResource resource)
        {
            IsMissing = false;
            _resource = resource;
            if (_resource != null) return;
            _resource = new LocaleStringResource();
            IsMissing = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleStringResourceModel"/> class.
        /// Initializing the class this way indicates that the resource is missing
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="resourceValue">The resource value.</param>
        public LocaleStringResourceModel(string resourceName, string resourceValue)
            : this(null)
        {
            ResourceName = resourceName;
            ResourceValue = resourceValue;
        }

        /// <summary>
        /// The name of the resource
        /// </summary>
        public string ResourceName
        {
            get { return _resource.ResourceName; }
            set
            {
                _resource.ResourceName = value;
                RaisePropertyChanged(() => ResourceName);
            }
        }

        /// <summary>
        /// The value of the resource
        /// </summary>
        public string ResourceValue
        {
            get { return _resource.ResourceValue; }
            set
            {
                _resource.ResourceValue = value;
                RaisePropertyChanged(() => ResourceValue);
            }
        }

        /// <summary>
        /// Is this value missing for the this language?
        /// </summary>
        public bool IsMissing { get; set; }
    }
}
