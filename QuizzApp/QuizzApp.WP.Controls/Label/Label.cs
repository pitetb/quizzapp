//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace QuizzApp.WP.Controls
{
    /// <summary>
    /// Displays a caption, required field indicator, and validation error indicator for a control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>    
    /// [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateNotRequired, GroupName = VisualStates.GroupRequired)]
    [TemplateVisualState(Name = VisualStates.StateRequired, GroupName = VisualStates.GroupRequired)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalid, GroupName = VisualStates.GroupValidation)]    
    public class Label : ContentControl
    {
        #region Member fields

        private bool _initialized;
        private bool _isRequiredOverridden;
        
        // Set to true the Content property is not directly set. The Content property can then
        // get its value from the potential ValidationMetadata.Caption.
        private bool _canContentUseMetaData;

        // Set to true when the Content property is not explicitely set by the developer
        private bool _isContentBeingSetInternally;

        private List<ValidationError> _errors;

        #endregion Member fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Label class.
        /// </summary>
        public Label()
        {
            this.DefaultStyleKey = typeof(Label);
            this._errors = new List<ValidationError>();

            // Set binding to self for DataContext change notifications
            this.SetBinding(Label.DataContextProperty, new Binding());
            this.Loaded += new RoutedEventHandler(this.Label_Loaded);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.Label_IsEnabledChanged);

            // Metadata can be consumed as long as the Content is still null.
            this._canContentUseMetaData = this.Content == null;

            if (DesignerProperties.IsInDesignTool)
            {
                this.SetContentInternally(typeof(Label).Name);
            }
        }

        #endregion Constructors

        #region Dependency Properties

        #region DataContext

        /// <summary>
        /// Identifies the DataContext dependency property.
        /// </summary>
        private static new readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(Label),
            new PropertyMetadata(OnDataContextPropertyChanged));

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label label = d as Label;
            if (label != null)
            {
                if (e.OldValue == null || e.NewValue == null || e.OldValue.GetType() != e.NewValue.GetType())
                {
                    // Refresh the metadata, but only if the DataContext's type has changed (or if either is null)
                    label.LoadMetadata(false);
                }
            }
        }

        #endregion DataContext

        #region IsRequired

        /// <summary>
        /// Identifies the IsRequired dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
            "IsRequired",
            typeof(bool),
            typeof(Label),
            new PropertyMetadata(OnIsRequiredPropertyChanged));

        /// <summary>
        ///   Gets or sets a value that indicates whether 
        ///   the <see cref="P:System.Windows.Controls.Label.Target" /> field is required. 
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return (bool)GetValue(Label.IsRequiredProperty);
            }

            set
            {
                this._isRequiredOverridden = true;
                SetValue(Label.IsRequiredProperty, value);
            }
        }

        /// <summary>
        /// Handle the IsRequired field property change event.
        /// </summary>
        /// <param name="depObj">The Label that had its IsRequired value changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsRequiredPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // Dependency property changed
            Label fl = depObj as Label;
            if (fl != null)
            {
                fl.UpdateRequiredState();
            }
        }

        #endregion IsRequired

        #region IsValid

        /// <summary>
        /// Identifies the IsValid dependency property
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(
            "IsValid",
            typeof(bool),
            typeof(Label),
            new PropertyMetadata(true, OnIsValidPropertyChanged));

        /// <summary>
        ///   Gets a value that indicates whether 
        ///   the <see cref="P:System.Windows.Controls.Label.Target" /> field data is valid. 
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            private set { this.SetValueNoCallback(IsValidProperty, value); }
        }

        private static void OnIsValidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label label = d as Label;
            if (label != null && !label.AreHandlersSuspended())
            {
                label.SetValueNoCallback(Label.IsValidProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "IsValid"));
            }
        }

        #endregion IsValid

        #region PropertyPath

        /// <summary>
        /// Identifies the PropertyPath dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty = DependencyProperty.Register(
            "PropertyPath",
            typeof(string),
            typeof(Label),
            new PropertyMetadata(OnPropertyPathPropertyChanged));

        /// <summary>
        ///   Gets or sets the path to the dependency property on the 
        ///   <see cref="P:System.Windows.FrameworkElement.DataContext" /> of the 
        ///   <see cref="P:System.Windows.Controls.Label.Target" /> control that this 
        ///   <see cref="T:System.Windows.Controls.Label" /> is associated with. 
        /// </summary>
        public string PropertyPath
        {
            get { return GetValue(Label.PropertyPathProperty) as string; }
            set { SetValue(Label.PropertyPathProperty, value); }
        }

        private static void OnPropertyPathPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Label label = depObj as Label;
            if (label != null && label.Initialized)
            {
                label.LoadMetadata(false);
                // Changing the PropertyPath sometimes requires an update for the validation state, since it might be stale.
                label.ParseTargetValidState();
            }
        }

        #endregion PropertyPath

        #region Target

        /// <summary>
        /// Identifies the Target dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(
            "Target",
            typeof(FrameworkElement),
            typeof(Label),
            new PropertyMetadata(OnTargetPropertyChanged));

        /// <summary>
        ///   Gets or sets the control that this <see cref="T:System.Windows.Controls.Label" /> is associated with. 
        /// </summary>
        public FrameworkElement Target
        {
            get { return GetValue(TargetProperty) as FrameworkElement; }
            set { SetValue(TargetProperty, value); }
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Label label = d as Label;
            if (label != null)
            {
                label.LoadMetadata(false);
                label._errors.Clear();
                FrameworkElement oldElement = e.OldValue as FrameworkElement;
                FrameworkElement newElement = e.NewValue as FrameworkElement;
                EventHandler<ValidationErrorEventArgs> bindingHandler = new EventHandler<ValidationErrorEventArgs>(label.Target_BindingValidationError);
                if (oldElement != null)
                {
                    oldElement.BindingValidationError -= bindingHandler;
                }
                if (newElement != null)
                {
                    newElement.BindingValidationError += bindingHandler;
                    ReadOnlyObservableCollection<ValidationError> newErrors = Validation.GetErrors(newElement);
                    if (newErrors.Count > 0)
                    {
                        // Only the first error is used by binding
                        label._errors.Add(newErrors[0]);
                    }
                }

                label.ParseTargetValidState();
            }
        }

        #endregion Target

        #endregion Dependency Properties

        #region Properties

        /// <summary>
        /// Internally get or set the ValidationMetadata.  
        /// </summary>
        internal ValidationMetadata ValidationMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the control has been initialized.
        /// </summary>
        internal bool Initialized
        {
            get { return this._initialized; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Set default states
            this.UpdateValidationState();
            this.UpdateRequiredState();
        }

        /// <summary>
        /// Reload the metadata from the source target or DataContext
        /// </summary>
        public virtual void Refresh()
        {
            this._isRequiredOverridden = false;
            this.LoadMetadata(true /*forceUpdate*/);
            this.ParseTargetValidState();
        }

        /// <summary>
        /// Called when the value of the System.Windows.Controls.Label.Content property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the System.Windows.Controls.Label.Content property.</param>
        /// <param name="newContent">The new value of the System.Windows.Controls.Label.Content property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (DesignerProperties.IsInDesignTool)
            {
                if (newContent == null)
                {
                    this.SetContentInternally(typeof(Label).Name);
                }
            }

            // The Content property can consume the metadata when the developer has not set 
            // it explicitely, or when it was set to null.
            this._canContentUseMetaData = this._isContentBeingSetInternally || newContent == null;
        }

        /// <summary>
        /// Used internally to set the Content property at design-time or based on metadata.
        /// </summary>
        /// <param name="value">new value for the Content property</param>
        private void SetContentInternally(object value)
        {
            try
            {
                this._isContentBeingSetInternally = true;
                this.Content = value;
            }
            finally
            {
                this._isContentBeingSetInternally = false;
            }
        }

        /// <summary>
        /// IsEnabled property change handler
        /// </summary>
        /// <param name="sender">The Label that had its IsEnabled value changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private void Label_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState();
        }

        /// <summary>
        /// Perform initialization code
        /// </summary>
        /// <param name="sender">The Label that has loaded.</param>
        /// <param name="e">The RoutedEventArgs for this event.</param>
        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._initialized)
            {
                // Loading Metadata onload because the dependency property could have changed before load and before the target was initialized
                this.LoadMetadata(false);
                this._initialized = true;
                this.Loaded -= new RoutedEventHandler(this.Label_Loaded);
            }
        }

        /// <summary>
        /// Load meta data and update the UI. 
        /// </summary>
        /// <param name="forceUpdate">If true, metadata will not be loaded from cache.</param>
        private void LoadMetadata(bool forceUpdate)
        {
            ValidationMetadata vmd = null;
            object entity = null;
            BindingExpression bindingExpression = null;
            if (!String.IsNullOrEmpty(this.PropertyPath))
            {
                entity = this.DataContext;
                // Pull metadata directly from the DataContext.  This isn't cached so it will be pulled every time.
                vmd = ValidationHelper.ParseMetadata(this.PropertyPath, entity);
            }
            else if (this.Target != null)
            {
                // Pull the metadata from the target FrameworkElement.  
                vmd = ValidationHelper.ParseMetadata(this.Target, forceUpdate, out entity, out bindingExpression);
            }
            if (this.ValidationMetadata != vmd)
            {
                this.ValidationMetadata = vmd;
                // Update to the new VMD
                if (this.ValidationMetadata != null)
                {
                    string newContent = this.ValidationMetadata.Caption;
                    if (newContent != null && this._canContentUseMetaData)
                    {
                        this.SetContentInternally(newContent);
                    }
                }
                else if (this._canContentUseMetaData)
                {
                    // The Target property was reset. Since the Content property
                    // was using the metadata, it also needs to be reset.
                    this.SetContentInternally(null);
                }
                
                if (!this._isRequiredOverridden)
                {
                    bool isRequired = this.ValidationMetadata == null ? false : this.ValidationMetadata.IsRequired;
                    SetValue(Label.IsRequiredProperty, isRequired);
                }
            }
        }

        /// <summary>
        /// Parse the target error state and update the IsValid property
        /// </summary>
        private void ParseTargetValidState()
        {
            this.IsValid = this._errors.Count == 0;
            this.UpdateValidationState();
        }

        /// <summary>
        /// Event handler for target control's BindingValidationError event.
        /// </summary>
        /// <param name="sender">The target that had a BindingValidationError event.</param>
        /// <param name="e">The ValidationErrorEventArgs for this event.</param>
        private void Target_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                if (!this._errors.Contains(e.Error))
                {
                    this._errors.Add(e.Error);
                }
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                if (this._errors.Contains(e.Error))
                {
                    this._errors.Remove(e.Error);
                }
            }

            this.ParseTargetValidState();
        }

        #region UpdateState

        /// <summary>
        /// Update the Common VSM state
        /// </summary>
        private void UpdateCommonState()
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? VisualStates.StateNormal : VisualStates.StateDisabled, true);
        }

        /// <summary>
        /// Update the required field VSM state based on the IsRequired property.  
        /// </summary>
        private void UpdateRequiredState()
        {
            VisualStateManager.GoToState(this, this.IsRequired ? VisualStates.StateRequired : VisualStates.StateNotRequired, true);
        }

        /// <summary>
        /// Update the validation VSM state
        /// </summary>
        private void UpdateValidationState()
        {
            VisualStateManager.GoToState(this, this.IsValid ? VisualStates.StateValid : VisualStates.StateInvalid, true);
        }

        #endregion UpdateState

        #endregion Methods
    }












    /// <summary>
    /// Encapsulates metadata info for a given property.
    /// </summary>
    internal class ValidationMetadata : INotifyPropertyChanged
    {
        private string _caption;
        private string _description;
        private bool _isRequired;

        /// <summary>
        /// Gets or sets a value indicating whether the field is required
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return this._isRequired;
            }

            set
            {
                if (this._isRequired != value)
                {
                    this._isRequired = value;
                    this.NotifyPropertyChanged("IsRequired");
                }
            }
        }

        /// <summary>
        /// Gets or sets the property description
        /// </summary>
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        public string Caption
        {
            get
            {
                return this._caption;
            }

            set
            {
                if (this._caption != value)
                {
                    this._caption = value;
                    this.NotifyPropertyChanged("Caption");
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// This event is raised when any of the properties on the object change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }











    internal class ValidationHelper
    {
        #region AttachedProperties

        #region ValidationMetadata

        /// <summary>
        /// Gets or sets the ValidationMetadata, which represents all of the metadata associated with the binding path of the input control.  This includes
        /// IsFieldRequired, RequiredFieldMessage, Caption, and Description.
        /// </summary>
        internal static readonly DependencyProperty ValidationMetadataProperty = DependencyProperty.RegisterAttached(
            "ValidationMetadata",
            typeof(ValidationMetadata),
            typeof(ValidationHelper),
            null);

        /// <summary>
        /// Gets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to get the ValidationMetadata property from.</param>
        /// <returns>The ValidationMetadata associated with the input control.</returns>
        internal static ValidationMetadata GetValidationMetadata(DependencyObject inputControl)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            return inputControl.GetValue(ValidationMetadataProperty) as ValidationMetadata;
        }

        /// <summary>
        /// Sets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to set the ValidationMetadata property on.</param>
        /// <param name="value">The ValidationMetadata to associate with the input control.</param>
        internal static void SetValidationMetadata(DependencyObject inputControl, ValidationMetadata value)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            inputControl.SetValue(ValidationMetadataProperty, value);
        }

        #endregion ValidationMetadata

        #endregion AttachedProperties

        #region Methods

        #region Static Methods

        /// <summary>
        /// Parse metadata from a target FrameworkElement.  This will cache the metadata on the element as an attached property.
        /// </summary>
        /// <param name="element">The target FrameworkElement to pull metadata from.</param>
        /// <param name="forceUpdate">If set, will not pull metadata from cache.</param>
        /// <param name="entity">The entity used.</param>
        /// <param name="bindingExpression">The bindingExpression used.</param>
        /// <returns>Returns the metadata associated with the element.  Will be null if no metadata was found.</returns>
        internal static ValidationMetadata ParseMetadata(FrameworkElement element, bool forceUpdate, out object entity, out BindingExpression bindingExpression)
        {
            entity = null;
            bindingExpression = null;
            if (element == null)
            {
                return null;
            }

            if (!forceUpdate)
            {
                ValidationMetadata existingVMD = element.GetValue(ValidationMetadataProperty) as ValidationMetadata;
                if (existingVMD != null)
                {
                    return existingVMD;
                }
            }

            BindingExpression be = null;
            FieldInfo[] fields = element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(DependencyProperty))
                {
                    // Found a dependency property
                    be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                    if (be != null && be.ParentBinding != null && be.ParentBinding.Path != null)
                    {
                        // Found a BindingExpression, ensure it has valid data
                        entity = be.DataItem != null ? be.DataItem : element.DataContext;
                        if (entity != null)
                        {
                            if (be.ParentBinding.Mode == BindingMode.TwoWay)
                            {
                                bindingExpression = be;
                                // A twoway binding will be automatically chosen and the rest ignored
                                break;
                            }

                            // Perform an arbitrary sort on path (string), so the same dependency property is chosen consistently.
                            // Reflection ordering is not deterministic and if we just pick the first, we could be 
                            // matched with different dependency properties depending on the run.
                            if (bindingExpression == null || string.Compare(be.ParentBinding.Path.Path, bindingExpression.ParentBinding.Path.Path, StringComparison.Ordinal) < 0)
                            {
                                bindingExpression = be;
                            }
                        }
                    }
                }
            }
            if (bindingExpression != null)
            {
                ValidationMetadata newVMD = ParseMetadata(bindingExpression.ParentBinding.Path.Path, entity);
                element.SetValue(ValidationMetadataProperty, newVMD);
                return newVMD;
            }
            return null;
        }

        /// <summary>
        /// Parse metadata given a binding path and entity object.
        /// </summary>
        /// <param name="bindingPath">The bindingPath is the name of the property on the entity from which to pull metadata from.  This supports dot notation.</param>
        /// <param name="entity">The entity object from which to pull metadata from.</param>
        /// <returns>The validation metadata associated with the entity and binding path.  This will return null if none exists.</returns>
        internal static ValidationMetadata ParseMetadata(string bindingPath, object entity)
        {
            if (entity != null && !String.IsNullOrEmpty(bindingPath))
            {
                Type entityType = entity.GetType();
                PropertyInfo prop = GetProperty(entityType, bindingPath);
                if (prop != null)
                {
                    ValidationMetadata newVMD = new ValidationMetadata();
                    object[] attributes = prop.GetCustomAttributes(false);
                    foreach (object propertyAttribute in attributes)
                    {
                        // Loop through each attribute and update the VMD as appropriate

                        // RequiredField
                        //RequiredAttribute reqAttribute = propertyAttribute as RequiredAttribute;
                        //if (reqAttribute != null)
                        //{
                        //    newVMD.IsRequired = true;
                        //    continue;
                        //}

                        //// Display attribute parsing
                        //DisplayAttribute displayAttribute = propertyAttribute as DisplayAttribute;
                        //if (displayAttribute != null)
                        //{
                        //    newVMD.Description = displayAttribute.GetDescription();
                        //    newVMD.Caption = displayAttribute.GetName();
                        //    continue;
                        //}
                    }
                    if (newVMD.Caption == null)
                    {
                        // If the name is not defined via the DisplayAttribute, use the property name. 
                        newVMD.Caption = prop.Name;

                        // Caption can be set to empty string to have an empty Caption and not default 
                        // to the property name.
                    }

                    return newVMD;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the PropertyInfo for a given entity type.  Supports dot notation to represent nested objects.
        /// </summary>
        /// <param name="entityType">The type of the object.</param>
        /// <param name="propertyPath">The property path, supporting dot notation.</param>
        /// <returns>The PropertyInfo corresponding to the final property.</returns>
        private static PropertyInfo GetProperty(Type entityType, string propertyPath)
        {
            Debug.Assert(entityType != null, "Unexpected null entityType in ValidationHelper.GetProperty");
            Debug.Assert(propertyPath != null, "Unexpected null propertyPath in ValidationHelper.GetProperty");
            Type itemType = entityType;

            string[] propertyNames = propertyPath.Split('.');
            if (propertyNames != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    PropertyInfo propertyInfo = itemType.GetProperty(propertyNames[i]);
                    if (propertyInfo == null || !propertyInfo.CanRead)
                    {
                        return null;
                    }

                    if (i == propertyNames.Length - 1)
                    {
                        return propertyInfo;
                    }
                    else
                    {
                        itemType = propertyInfo.PropertyType;
                    }
                }
            }
            return null;
        }

        #endregion Static Methods

        #endregion Methods
    }






    /// <summary>
    /// Reservoir of attached properties for use by extension methods that require non-static information about objects.
    /// </summary>
    internal class ExtensionProperties : DependencyObject
    {
        /// <summary>
        /// Tracks whether or not the event handlers of a particular object are currently suspended.
        /// Used by the SetValueNoCallback and AreHandlersSuspended extension methods.
        /// </summary>
        public static readonly DependencyProperty AreHandlersSuspended = DependencyProperty.RegisterAttached(
            "AreHandlersSuspended",
            typeof(Boolean),
            typeof(ExtensionProperties),
            new PropertyMetadata(false)
        );
        public static void SetAreHandlersSuspended(DependencyObject obj, Boolean value)
        {
            obj.SetValue(AreHandlersSuspended, value);
        }
        public static Boolean GetAreHandlersSuspended(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(AreHandlersSuspended);
        }
    }

    /// <summary>
    /// Utility class for operations.
    /// </summary>
    internal static class Extensions
    {
        #region Static Methods

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            ExtensionProperties.SetAreHandlersSuspended(obj, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                ExtensionProperties.SetAreHandlersSuspended(obj, false);
            }
        }

        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return ExtensionProperties.GetAreHandlersSuspended(obj);
        }

        #endregion Static Methods
    }

}
