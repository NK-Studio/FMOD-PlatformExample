using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace NKStudio.UIElements
{
    [MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
    public class SimpleBaseField : BindableElement
    {
        public new class UxmlFactory : UxmlFactory<SimpleBaseField, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _labelAttribute = new UxmlStringAttributeDescription
            {
                name = "Label", defaultValue = "Title"
            };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((SimpleBaseField)ve).Label = _labelAttribute.GetValueFromBag(bag, cc);
            }
        }

        private float _labelWidthRatio;
        private float _labelBaseMinWidth;
        private float _labelExtraPadding;
        private float _labelExtraContextWidth;

        private VisualElement _cachedContextWidthElement;
        private VisualElement _cachedInspectorElement;

        private static CustomStyleProperty<float> s_LabelWidthRatioProperty =
            new("--unity-property-field-label-width-ratio");

        private static CustomStyleProperty<float> s_LabelExtraPaddingProperty =
            new("--unity-property-field-label-extra-padding");

        private static CustomStyleProperty<float> s_LabelBaseMinWidthProperty =
            new("--unity-property-field-label-base-min-width");

        private static CustomStyleProperty<float> s_LabelExtraContextWidthProperty =
            new("--unity-base-field-extra-context-width");

        private Label _label;
        private VisualElement _container;

        public string Label
        {
            get => _label.text;
            set => _label.text = value;
        }

        public SimpleBaseField()
        {
            // ------------- Create
            _label = new Label();
            _container = new VisualElement
            {
                name = "unity-content"
            };
            // ------------- Style
            style.flexDirection = FlexDirection.Row;
            _container.style.flexDirection = FlexDirection.Row;
            _container.style.alignItems = Align.Center;
            
            _label.style.marginRight = 0;
            SetupUnityBaseTextFieldWithoutColor(ref _container);

            _label.text = "Label";
            // ------------- Class
            AddToClassList("unity-base-field__aligned");
            AddToClassList("unity-base-field");
            AddToClassList("unity-base-field__inspector-field");
            AddToClassList("unity-base-text-field");
            AddToClassList("unity-text-field");

            _label.AddToClassList("unity-label");
            _label.AddToClassList("unity-text-element");
            _label.AddToClassList("unity-text-field__label");
            _label.AddToClassList("unity-base-field__label");
            _label.AddToClassList("unity-base-text-field__label");

            _container.AddToClassList("unity-base-field__input");
            _container.AddToClassList("unity-base-text-field__input--single-line");
            
            
            _container.style.paddingLeft = 0;
            _container.style.paddingRight = 0;
            _container.style.borderLeftWidth = 0;
            _container.style.borderRightWidth = 0;
            // ------------- Add
            hierarchy.Add(_label);
            hierarchy.Add(_container);
            // ------------- Event
            RegisterCallback(new EventCallback<AttachToPanelEvent>(OnAttachToPanel));
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            if (e.destinationPanel == null || e.destinationPanel.contextType == ContextType.Player)
                return;
            for (VisualElement parent = this.parent; parent != null; parent = parent.parent)
            {
                if (parent.ClassListContains("unity-inspector-element"))
                    _cachedInspectorElement = parent;
                if (parent.ClassListContains("unity-inspector-main-container"))
                {
                    _cachedContextWidthElement = parent;
                    break;
                }
            }

            if (_cachedInspectorElement == null)
                return;
            _labelWidthRatio = 0.45f;
            _labelExtraPadding = 37f;
            _labelBaseMinWidth = 123f;
            _labelExtraContextWidth = 1f;
            RegisterCallback(new EventCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved));
            RegisterCallback(new EventCallback<GeometryChangedEvent>(OnInspectorFieldGeometryChanged));
        }

        private void AlignLabel()
        {
            if (!ClassListContains("unity-base-field__aligned"))
                return;

            float labelExtraPadding = _labelExtraPadding;
            Rect worldBound = this.worldBound;
            double x1 = worldBound.x;
            worldBound = _cachedInspectorElement.worldBound;
            double x2 = worldBound.x;
            float num1 = (float)(x1 - x2) - _cachedInspectorElement.resolvedStyle.paddingLeft;
            float num2 = labelExtraPadding + num1 + resolvedStyle.paddingLeft;
            float a = _labelBaseMinWidth - num1 - resolvedStyle.paddingLeft;
            VisualElement visualElement = _cachedContextWidthElement ?? _cachedInspectorElement;
            _label.style.minWidth = Mathf.Max(a, 0.0f);
            float b = (visualElement.resolvedStyle.width + _labelExtraContextWidth) * _labelWidthRatio - num2;
            if (Mathf.Abs(_label.resolvedStyle.width - b) <= 1.0000000031710769E-30)
                return;
            _label.style.width = Mathf.Max(0.0f, b);
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(s_LabelWidthRatioProperty, out var num1))
                _labelWidthRatio = num1;
            if (evt.customStyle.TryGetValue(s_LabelExtraPaddingProperty, out var num2))
                _labelExtraPadding = num2;
            if (evt.customStyle.TryGetValue(s_LabelBaseMinWidthProperty, out var num3))
                _labelBaseMinWidth = num3;
            if (evt.customStyle.TryGetValue(s_LabelExtraContextWidthProperty, out var num4))
                _labelExtraContextWidth = num4;
            AlignLabel();
        }

        private void OnInspectorFieldGeometryChanged(GeometryChangedEvent e) => AlignLabel();

        private void SetupUnityBaseTextFieldWithoutColor(ref VisualElement container)
        {
            container.style.paddingLeft = 2;
            container.style.paddingRight = 2;
            container.style.paddingTop = 1;
            container.style.paddingBottom = 0;
            container.style.flexShrink = 1;
            container.style.flexGrow = 1;
            container.style.borderLeftWidth = 1;
            container.style.borderRightWidth = 1;
            container.style.borderTopWidth = 1;
            container.style.borderBottomWidth = 1;
        }

        public override VisualElement contentContainer => _container;
    }
}