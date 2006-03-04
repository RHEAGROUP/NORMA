#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
*                                                                          *
* The use and distribution terms for this software are covered by the      *
* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
* can be found in the file CPL.txt at the root of this distribution.       *
* By using this software in any fashion, you are agreeing to be bound by   *
* the terms of this license.                                               *
*                                                                          *
* You must not remove this notice, or any other, from this software.       *
\**************************************************************************/
#endregion

using System;
using System.Diagnostics;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORM.ShapeModel;
using System.Resources;
using System.Windows.Forms;

namespace Neumont.Tools.ORM
{
	/// <summary>
	/// A constant list of strings corresponding to resource identifiers
	/// in the resource files for all models. Any resource id referenced
	/// directly in non-spit code should be duplicated here.
	/// </summary>
	internal static partial class ResourceStrings
	{
		#region Supported Resource Managers
		/// <summary>
		/// Recognized resource managers
		/// </summary>
		private enum ResourceManagers
		{
			/// <summary>
			/// IMS-managed resource file for the core object model
			/// </summary>
			ObjectModel,
			/// <summary>
			/// IMS-managed resource file for the shape object model
			/// </summary>
			ShapeModel,
			/// <summary>
			/// Standalone resource file for the core model
			/// </summary>
			Model,
			/// <summary>
			/// Standalone resource file for the diagram
			/// </summary>
			Diagram,
		}
		#endregion // Supported Resource Managers
		#region Non-IMS ResourceManagers
		private static object myLockObject;
		private static object LockObject
		{
			get
			{
				if (myLockObject == null)
				{
					System.Threading.Interlocked.CompareExchange(ref myLockObject, new object(), null);
				}
				return myLockObject;
			}
		}
		private static ResourceManager myDiagramResourceManager;
		private static ResourceManager DiagramResourceManager
		{
			get
			{
				if (myDiagramResourceManager == null)
				{
					lock (LockObject)
					{
						if (myDiagramResourceManager == null)
						{
							myDiagramResourceManager = LoadResourceManagerForType(typeof(ORMDiagram));
						}
					}
				}
				return myDiagramResourceManager;
			}
		}
		private static ResourceManager LoadResourceManagerForType(Type type)
		{
			return new ResourceManager(type.FullName, type.Assembly);
		}

		private static ResourceManager myModelResourceManager;
		private static ResourceManager ModelResourceManager
		{
			get
			{
				if (myModelResourceManager == null)
				{
					lock (LockObject)
					{
						if (myModelResourceManager == null)
						{
							myModelResourceManager = LoadResourceManagerForType(typeof(ORMModel));
						}
					}
				}
				return myModelResourceManager;
			}
		}
		#endregion // Non-IMS ResourceManagers
		#region Helper functions
		private static ResourceManager GetResourceManager(ResourceManagers manager)
		{
			ResourceManager resMgr = null;
			switch (manager)
			{
				case ResourceManagers.ObjectModel:
					resMgr = ORMMetaModel.SingletonResourceManager;
					break;
				case ResourceManagers.ShapeModel:
					resMgr = ORMShapeModel.SingletonResourceManager;
					break;
				case ResourceManagers.Diagram:
					resMgr = DiagramResourceManager;
					break;
				case ResourceManagers.Model:
					resMgr = ModelResourceManager;
					break;
			}
			return resMgr;
		}

		private static string GetString(ResourceManagers manager, string resourceName)
		{
			ResourceManager resMgr = null;
			string retVal = null;
			resMgr = GetResourceManager(manager);
			if (resMgr != null)
			{
				retVal = resMgr.GetString(resourceName);
			}
			Debug.Assert(retVal != null && retVal.Length > 0, "Unrecognized resource string: " + resourceName);
			return (retVal != null) ? retVal : String.Empty;
		}

		private static object GetObject(ResourceManagers manager, string resourceName)
		{
			ResourceManager resMgr = null;
			object retVal = null;
			resMgr = GetResourceManager(manager);
			if (resMgr != null)
			{
				retVal = resMgr.GetObject(resourceName);
			}
			Debug.Assert(retVal != null, "Unrecognized resource string: " + resourceName);
			return retVal;
		}
		#endregion // Helper functions
		#region Public resource ids
		/// <summary>
		/// The identifier for the EntityType toolbox item
		/// </summary>
		public const string ToolboxEntityTypeItemId = "Toolbox.EntityType.Item.Id";
		/// <summary>
		/// The identifier for the ValueType toolbox item
		/// </summary>
		public const string ToolboxValueTypeItemId = "Toolbox.ValueType.Item.Id";
		/// <summary>
		/// The identifier for the ObjectifiedFactType toolbox item
		/// </summary>
		public const string ToolboxObjectifiedFactTypeItemId = "Toolbox.ObjectifiedFactType.Item.Id";
		/// <summary>
		/// The identifier for the UnaryFactType toolbox item
		/// </summary>
		public const string ToolboxUnaryFactTypeItemId = "Toolbox.UnaryFactType.Item.Id";
		/// <summary>
		/// The identifier for the BinaryFactType toolbox item
		/// </summary>
		public const string ToolboxBinaryFactTypeItemId = "Toolbox.BinaryFactType.Item.Id";
		/// <summary>
		/// The identifier for the TernaryFactType toolbox item
		/// </summary>
		public const string ToolboxTernaryFactTypeItemId = "Toolbox.TernaryFactType.Item.Id";
		/// <summary>
		/// The identifier for an ExternalUniquenessConstraint toolbox item
		/// </summary>
		public const string ToolboxExternalUniquenessConstraintItemId = "Toolbox.ExternalUniquenessConstraint.Item.Id";
		/// <summary>
		/// The identifier for an InternalUniquenessConstraint toolbox item
		/// </summary>
		public const string ToolboxInternalUniquenessConstraintItemId = "Toolbox.InternalUniquenessConstraint.Item.Id";
		/// <summary>
		/// The identifier for an ExclusionConstraint toolbox item
		/// </summary>
		public const string ToolboxExclusionConstraintItemId = "Toolbox.ExclusionConstraint.Item.Id";
		/// <summary>
		/// The identifier for an InclusiveOrConstraint toolbox item
		/// </summary>
		public const string ToolboxInclusiveOrConstraintItemId = "Toolbox.InclusiveOrConstraint.Item.Id";
		/// <summary>
		/// The identifier for an ExclusiveOrConstraint toolbox item
		/// </summary>
		public const string ToolboxExclusiveOrConstraintItemId = "Toolbox.ExclusiveOrConstraint.Item.Id";
		/// <summary>
		/// The identifier for the RoleConnector toolbox item
		/// </summary>
		public const string ToolboxRoleConnectorItemId = "Toolbox.RoleConnector.Item.Id";
		/// <summary>
		/// The identifier for a FrequencyConstraint toolbox item
		/// </summary>
		public const string ToolboxFrequencyConstraintItemId = "Toolbox.FrequencyConstraint.Item.Id";
		/// <summary>
		/// The identifier for an SubsetConstraint toolbox item
		/// </summary>
		public const string ToolboxSubsetConstraintItemId = "Toolbox.SubsetConstraint.Item.Id";
		/// <summary>
		/// The identifier for an EqualityConstraint toolbox item
		/// </summary>
		public const string ToolboxEqualityConstraintItemId = "Toolbox.EqualityConstraint.Item.Id";
		/// <summary>
		/// The identifier for an ExternalConstraintConnector toolbox item
		/// </summary>
		public const string ToolboxExternalConstraintConnectorItemId = "Toolbox.ExternalConstraintConnector.Item.Id";
		/// <summary>
		/// The identifier for an SubtypeConnector toolbox item
		/// </summary>
		public const string ToolboxSubtypeConnectorItemId = "Toolbox.SubtypeConnector.Item.Id";
		/// <summary>
		/// The identifier for an InternalUniquenessConstraintConnector toolbox item
		/// </summary>
		public const string ToolboxInternalUniquenessConstraintConnectorItemId = "Toolbox.InternalUniquenessConstraintConnector.Item.Id";
		/// <summary>
		/// The identifier for a Ring Constraint
		/// </summary>
		public const string ToolboxRingConstraintItemId = "Toolbox.RingConstraint.Item.Id";
		/// <summary>
		/// Category name for options page (appearance)
		/// </summary>
		public const string OptionsPageCategoryAppearanceId = "OptionsPage.Category.Appearance";
		/// <summary>
		/// Category name for options page (data type)
		/// </summary>
		public const string OptionsPageCategoryDataTypesId = "OptionsPage.Category.DataTypes";
		/// <summary>
		/// Category name for options page (delete behavior)
		/// </summary>
		public const string OptionsPageCategoryDeleteBehaviorId = "OptionsPage.Category.DeleteBehavior";
		/// <summary>
		/// Description of the default data type option
		/// </summary>
		public const string OptionsPagePropertyDataTypeDescriptionId = "OptionsPage.Property.DataType.Description";
		/// <summary>
		/// Display Name of the default data type option
		/// </summary>
		public const string OptionsPagePropertyDataTypeDisplayNameId = "OptionsPage.Property.DataType.DisplayName";
		/// <summary>
		/// Description of the external constraint role bar display option
		/// </summary>
		public const string OptionsPagePropertyExternalConstraintRoleBarDisplayDescriptionId = "OptionsPage.Property.ExternalConstraintRoleBarDisplay.Description";
		/// <summary>
		/// Display name of the external constraint role bar display option
		/// </summary>
		public const string OptionsPagePropertyExternalConstraintRoleBarDisplayNameId = "OptionsPage.Property.ExternalConstraintRoleBarDisplay.DisplayName";
		/// <summary>
		/// Description of the object type shape
		/// </summary>
		public const string OptionsPagePropertyObjectTypeShapeDescriptionId = "OptionsPage.Property.ObjectTypeShape.Description";
		/// <summary>
		/// Display name of the object type shape
		/// </summary>
		public const string OptionsPagePropertyObjectTypeShapeDisplayNameId = "OptionsPage.Property.ObjectTypeShape.DisplayName";
		/// <summary>
		/// Description of the objectified fact shape
		/// </summary>
		public const string OptionsPagePropertyObjectifiedShapeDescriptionId = "OptionsPage.Property.ObjectifiedShape.Description";
		/// <summary>
		/// Display name of the objectified fact shape
		/// </summary>
		public const string OptionsPagePropertyObjectifiedShapeDisplayNameId = "OptionsPage.Property.ObjectifiedShape.DisplayName";
		/// <summary>
		/// Description of the Mandatory Dot placement
		/// </summary>
		public const string OptionsPagePropertyMandatoryDotDescriptionId = "OptionsPage.Property.MandatoryDot.Description";
		/// <summary>
		/// Display name of the Mandatory Dot placement
		/// </summary>
		public const string OptionsPagePropertyMandatoryDotDisplayNameId = "OptionsPage.Property.MandatoryDot.DisplayName";
		/// <summary>
		/// Description of the Role Name Display option
		/// </summary>
		public const string OptionsPagePropertyRoleNameDisplayDescriptionId = "OptionsPage.Property.RoleNameDisplay.Description";
		/// <summary>
		/// Display Name of the Role Name Display option
		/// </summary>
		public const string OptionsPagePropertyRoleNameDisplayDisplayNameId = "OptionsPage.Property.RoleNameDisplay.DisplayName";
		/// <summary>
		/// Description of the Primary Delete Behavior
		/// </summary>
		public const string OptionsPagePropertyPrimaryDeleteBehaviorDescriptionId = "OptionsPage.Property.PrimaryDeleteBehavior.Description";
		/// <summary>
		/// Display Name of the Primary Delete Behavior option
		/// </summary>
		public const string OptionsPagePropertyPrimaryDeleteBehaviorDisplayNameId = "OptionsPage.Property.PrimaryDeleteBehavior.DisplayName";
		/// <summary>
		/// Description of the Final Shape Delete Behavior option
		/// </summary>
		public const string OptionsPagePropertyFinalShapeDeleteBehaviorDescriptionId = "OptionsPage.Property.FinalShapeDeleteBehavior.Description";
		/// <summary>
		/// Display Name of the Final Shape Delete Behavior option
		/// </summary>
		public const string OptionsPagePropertyFinalShapeDeleteBehaviorDisplayNameId = "OptionsPage.Property.FinalShapeDeleteBehavior.DisplayName";
		/// <summary>
		/// Display Name of the Object Name color
		/// </summary>
		public const string FactEditorColorsObjectNameId = "FactEditorColors.ObjectName";
		/// <summary>
		/// Display Name of the Reference Mode Name color
		/// </summary>
		public const string FactEditorColorsReferenceModeNameId = "FactEditorColors.ReferenceModeName";
		/// <summary>
		/// Display Name of the Predicate Text color
		/// </summary>
		public const string FactEditorColorsPredicateTextId = "FactEditorColors.PredicateText";
		/// <summary>
		/// Display Name of the Delimiter color
		/// </summary>
		public const string FactEditorColorsDelimiterId = "FactEditorColors.Delimiter";
		/// <summary>
		/// Display name for the ORM Designer fonts and colors category
		/// </summary>
		public const string FontsAndColorsEditorCategoryNameId = "FontsAndColors.EditorCategoryName";
		/// <summary>
		/// Display name for the ORM Verbalizer fonts and colors category
		/// </summary>
		public const string FontsAndColorsVerbalizerCategoryNameId = "FontsAndColors.VerbalizerCategoryName";
		/// <summary>
		/// Display name for the color used to draw an ORM Role Name
		/// </summary>
		public const string FontsAndColorsRoleNameColorId = "FontsAndColors.RoleNameColor";
		/// <summary>
		/// Display name for the color used to draw an ORM constraint
		/// </summary>
		public const string FontsAndColorsConstraintColorId = "FontsAndColors.ConstraintColor";
		/// <summary>
		/// Display name for the color used to draw an ORM constraint with deontic modality
		/// </summary>
		public const string FontsAndColorsDeonticConstraintColorId = "FontsAndColors.DeonticConstraintColor";
		/// <summary>
		/// Display name for the color used to draw an ORM constraint
		/// </summary>
		public const string FontsAndColorsConstraintErrorColorId = "FontsAndColors.ConstraintErrorColor";
		/// <summary>
		/// Display name for the color used to draw an active ORM constraint and associated roles
		/// </summary>
		public const string FontsAndColorsActiveConstraintColorId = "FontsAndColors.ActiveConstraintColor";
		/// <summary>
		/// Display name for the color used to draw the constraint box for role sequence editing
		/// </summary>
		public const string FontsAndColorsRolePickerColorId = "FontsAndColors.RolePickerColor";
		/// <summary>
		/// Display name for the color used to draw predicate text in the verbalizer
		/// </summary>
		public const string FontsAndColorsVerbalizerPredicateTextColorId = "FontsAndColors.VerbalizerPredicateTextColor";
		/// <summary>
		/// Display name for the color used to draw object names in the verbalizer
		/// </summary>
		public const string FontsAndColorsVerbalizerObjectNameColorId = "FontsAndColors.VerbalizerObjectNameColor";
		/// <summary>
		/// Display name for the color used to draw formal items in the verbalizer
		/// </summary>
		public const string FontsAndColorsVerbalizerFormalItemColorId = "FontsAndColors.VerbalizerFormalItemColor";
		/// <summary>
		/// Display Name of the Quantifier color
		/// </summary>
		public const string FactEditorColorsQuantifierId = "FactEditorColors.Quantifier";
		#endregion // Public resource ids
		#region Private resource ids
		private const string FactEditorIntellisenseImageList_Id = "FactEditor.Intellisense.ImageList";
		private const string DiagramTabImage_Id = "Diagram.TabImage";
		#endregion // Private resource ids
		#region Public accessor properties
		/// <summary>
		/// The category name to display on the options pages
		/// </summary>
		public static string GetOptionsPageString(string resourceName)
		{
			return GetString(ResourceManagers.Diagram, resourceName);
		}
		/// <summary>
		/// The localized string to display on Fonts and Colors setting
		/// for the ORM Designer in the options page.
		/// </summary>
		public static string GetColorNameString(string resourceName)
		{
			return GetString(ResourceManagers.Diagram, resourceName);
		}
		/// <summary>
		/// The images for the Intellisense drop down
		/// </summary>
		public static ImageListStreamer FactEditorIntellisenseImageList
		{
			get
			{
				return GetObject(ResourceManagers.Diagram, FactEditorIntellisenseImageList_Id) as ImageListStreamer;
			}
		}
		/// <summary>
		/// The <see cref="System.Drawing.Bitmap"/> displayed on <see cref="Shell.ORMDesignerDocView"/> tabs for
		/// <see cref="ORMDiagram"/>s.
		/// </summary>
		public static System.Drawing.Bitmap DiagramTabImage
		{
			get
			{
				return GetObject(ResourceManagers.Diagram, DiagramTabImage_Id) as System.Drawing.Bitmap;
			}
		}
		#endregion // Public accessor properties
	}
}
