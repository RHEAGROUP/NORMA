#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright © Neumont University. All rights reserved.                     *
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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Neumont.Tools.ORM.ObjectModel;
using Neumont.Tools.ORM.ShapeModel;
using Neumont.Tools.ORM.Shell;
using System.Globalization;
using CategoryAttribute = System.ComponentModel.CategoryAttribute;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
namespace Neumont.Tools.ORM.Shell
{
	#region Shape enums
	/// <summary>
	/// Valid shapes for object types
	/// </summary>
	public enum ObjectTypeDisplayShape
	{
		/// <summary>
		/// Draw object shapes with soft rectangles
		/// </summary>
		SoftRectangle,
		/// <summary>
		/// Draw object shapes with hard rectangles
		/// </summary>
		HardRectangle,
		/// <summary>
		/// Draw object shapes with ellipses
		/// </summary>
		Ellipse,
	}

	/// <summary>
	/// Valid shapes for objectified facts
	/// </summary>
	public enum ObjectifiedFactDisplayShape
	{
		/// <summary>
		/// Draw objectified facts with soft rectangles
		/// </summary>
		SoftRectangle,
		/// <summary>
		/// Draw objectified facts with hard rectangles
		/// </summary>
		HardRectangle,
	}

	/// <summary>
	/// Valid placements for mandatory dots
	/// </summary>
	public enum MandatoryDotPlacement
	{
		/// <summary>
		/// Place the mandatory dot at the object shape end of the connector
		/// </summary>
		ObjectShapeEnd,
		/// <summary>
		/// Place the mandatory dot at the role box end of the connector
		/// </summary>
		RoleBoxEnd,
	}
	/// <summary>
	/// Determines when to display external constraint attach
	/// point bars.
	/// </summary>
	public enum ExternalConstraintRoleBarDisplay
	{
		/// <summary>
		/// Only display a bar if one or more roles in
		/// the set of associated roles have a role in between them.
		/// This is the default setting.
		/// </summary>
		SplitRoles,
		/// <summary>
		/// Display a bar if there is more than one role, even
		/// if the roles are adjacent.
		/// </summary>
		AdjacentRoles,
		/// <summary>
		/// Display a bar even if there is only a single role.
		/// This is useful for navigation with keyboard and
		/// accessibility readers because it guarantees that all
		/// constraints attached to a fact have an associated
		/// visible and selectable item in a constraint box.
		/// </summary>
		AnyRole,
	}
	/// <summary>
	/// Determine when an internal uniqueness constraint being
	/// that is a preferred identifier displays as a double line.
	/// These options are cumulative, so any higher display option
	/// turns on all other displays.
	/// </summary>
	public enum PreferredInternalUniquenessConstraintDisplay
	{
		/// <summary>
		/// Never display as preferred
		/// </summary>
		Never,
		/// <summary>
		/// Display as preferred when the fact is not
		/// explicitly or implicitly objectified. This only
		/// occurs with single-role uniqueness constraints
		/// on binary facts.
		/// </summary>
		UnobjectifiedInternalConstraint,
		/// <summary>
		/// Display as preferred when the fact is explicitly
		/// objectified and has multiple internal uniqueness constraints.
		/// </summary>
		MultipleObjectifiedInternalConstraints,
		/// <summary>
		/// Display as preferred when the fact is explicitly
		/// objectified and has a single internal constraint.
		/// </summary>
		SingleObjectifiedInternalConstraint,
		/// <summary>
		/// Display as preferred when the fact is implicitly
		/// objectified and has multiple internal uniqueness constraints.
		/// Implicit objectification occurs for n-aries and binaries with
		/// a spanning internal uniqueness constraint, so this will generally
		/// apply only to n-ary facts with more than one uniqueness constraint.
		/// </summary>
		MultipleImpliedObjectifiedInternalConstraints,
		/// <summary>
		/// Display as preferred when the fact is implicitly
		/// objectified and has a single internal constraint.
		/// Implicit objectification occurs for n-aries and binaries with
		/// a spanning internal uniqueness constraint and will draw
		/// spanning internal uniqueness constraints on binaries as preferred.
		/// </summary>
		SingleImpliedObjectifiedInternalConstraint,
	}
	#endregion // Shape enums
	#region Other Options Enums
	/// <summary>
	/// Provide options for showing and hiding role names on object types
	/// </summary>
	public enum RoleNameDisplay
	{
		/// <summary>
		/// Show role names
		/// </summary>
		On,
		/// <summary>
		/// Hide role names
		/// </summary>
		Off,
	}
	/// <summary>
	/// Provide options for the behavior of the Delete and Control-Delete keys
	/// </summary>
	public enum PrimaryDeleteBehavior
	{
		/// <summary>
		/// The Delete key deletes shapes, Ctrl-Delete deletes elements from
		/// the model
		/// </summary>
		DeleteShape,
		/// <summary>
		/// The Delete key deletes elements from the  model, Ctrl-Delete
		/// deletes shapes from the current diagram
		/// </summary>
		DeleteElement,
	}
	/// <summary>
	/// Provide options for the designer behavior when the final shape for
	/// an element is deleted from a diagram.
	/// </summary>
	public enum FinalShapeDeleteBehavior
	{
		/// <summary>
		/// Do not delete the underlying element from the object model
		/// </summary>
		DeleteShapeOnly,
		/// <summary>
		/// Delete the shape and the underlying element
		/// </summary>
		DeleteShapeAndElement,
		/// <summary>
		/// Ask the user if they would like to delete the underlying element
		/// </summary>
		Prompt,
	}
	#endregion
	#region NotifyDocument Delegate
	/// <summary>
	/// Used as a callback delegate for the OptionsPage.NotifySettingsChange
	/// method. Allow other global notification methods to walk all open
	/// documents.
	/// </summary>
	/// <param name="docData">A running ORMDesignerDocData to modify</param>
	[CLSCompliant(false)]
	public delegate void NotifyDocument(ORMDesignerDocData docData);
	#endregion // NotifyDocument Delegate
	/// <summary>
	/// Options dialog for ORM designers
	/// see https://projects.neumont.edu/orm2/wiki/HowToAddOptionPageOptions for adding options
	/// </summary>
	[CLSCompliant(false)]
	[Guid("B4ABD9FD-CE79-4B26-8D36-F345CB53B525")]
	public class OptionsPage : DialogPage
	{
		#region Localized PropertyDescriptor attribute classes
		private sealed class LocalizedCategoryAttribute : CategoryAttribute
		{
			public LocalizedCategoryAttribute(string category) : base(category)
			{
			}
			protected override string GetLocalizedString(string value)
			{
				return ResourceStrings.GetOptionsPageString(value);
			}
		}
		private sealed class LocalizedDescriptionAttribute : DescriptionAttribute
		{
			public LocalizedDescriptionAttribute(string description) : base(description)
			{
			}

			public override string Description
			{
				get { return ResourceStrings.GetOptionsPageString(base.Description); }
			}
		}
		private sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
		{
			public LocalizedDisplayNameAttribute(string displayName) : base(displayName)
			{
			}
			public override string DisplayName
			{
				get { return ResourceStrings.GetOptionsPageString(base.DisplayName); }
			}
		}
		#endregion // Localized PropertyDescriptor attribute classes
		#region Member variables
		// If more settings are added, add a corresponding check in the OnApply override below
		private const ObjectTypeDisplayShape ObjectTypeDisplayShape_Default = ObjectTypeDisplayShape.SoftRectangle;
		private static ObjectTypeDisplayShape myCurrentObjectTypeDisplayShape = ObjectTypeDisplayShape_Default;
		private ObjectTypeDisplayShape myObjectTypeDisplayShape = ObjectTypeDisplayShape_Default;

		private const ObjectifiedFactDisplayShape ObjectifiedFactDisplayShape_Default = ObjectifiedFactDisplayShape.SoftRectangle;
		private static ObjectifiedFactDisplayShape myCurrentObjectifiedFactDisplayShape = ObjectifiedFactDisplayShape_Default;
		private ObjectifiedFactDisplayShape myObjectifiedFactDisplayShape = ObjectifiedFactDisplayShape_Default;

		private const MandatoryDotPlacement MandatoryDotPlacement_Default = MandatoryDotPlacement.RoleBoxEnd;
		private static MandatoryDotPlacement myCurrentMandatoryDotPlacement = MandatoryDotPlacement_Default;
		private MandatoryDotPlacement myMandatoryDotPlacement = MandatoryDotPlacement_Default;

		private const RoleNameDisplay RoleNameDisplay_Default = RoleNameDisplay.On;
		private static RoleNameDisplay myCurrentRoleNameDisplay = RoleNameDisplay_Default;
		private RoleNameDisplay myRoleNameDisplay = RoleNameDisplay_Default;

		private const PortableDataType DefaultDataType_Default = PortableDataType.Unspecified;
		private static PortableDataType myCurrentDefaultDataType = DefaultDataType_Default;
		private PortableDataType myDefaultDataType = DefaultDataType_Default;

		private const ExternalConstraintRoleBarDisplay ExternalConstraintRoleBarDisplay_Default = ExternalConstraintRoleBarDisplay.SplitRoles;
		private static ExternalConstraintRoleBarDisplay myCurrentExternalConstraintRoleBarDisplay = ExternalConstraintRoleBarDisplay_Default;
		private ExternalConstraintRoleBarDisplay myExternalConstraintRoleBarDisplay = ExternalConstraintRoleBarDisplay_Default;

		private const PrimaryDeleteBehavior PrimaryDeleteBehavior_Default = PrimaryDeleteBehavior.DeleteShape;
		private static PrimaryDeleteBehavior myCurrentPrimaryDeleteBehavior = PrimaryDeleteBehavior_Default;
		private PrimaryDeleteBehavior myPrimaryDeleteBehavior = PrimaryDeleteBehavior_Default;

		private const FinalShapeDeleteBehavior FinalShapeDeleteBehavior_Default = FinalShapeDeleteBehavior.Prompt;
		private static FinalShapeDeleteBehavior myCurrentFinalShapeDeleteBehavior = FinalShapeDeleteBehavior_Default;
		private FinalShapeDeleteBehavior myFinalShapeDeleteBehavior = FinalShapeDeleteBehavior_Default;

		private const bool CombineMandatoryAndUniqueVerbalization_Default = true;
		private static bool myCurrentCombineMandatoryAndUniqueVerbalization = CombineMandatoryAndUniqueVerbalization_Default;
		private bool myCombineMandatoryAndUniqueVerbalization = CombineMandatoryAndUniqueVerbalization_Default;

		private const bool ShowDefaultConstraintVerbalization_Default = true;
		private static bool myCurrentShowDefaultConstraintVerbalization = ShowDefaultConstraintVerbalization_Default;
		private bool myShowDefaultConstraintVerbalization = ShowDefaultConstraintVerbalization_Default;

		private const string CustomVerbalizationSnippets_Default = "";
		private static string myCurrentCustomVerbalizationSnippets = CustomVerbalizationSnippets_Default;
		private string myCustomVerbalizationSnippets = CustomVerbalizationSnippets_Default;

		private const PreferredInternalUniquenessConstraintDisplay PreferredInternalUniquenessConstraintDisplay_Default = PreferredInternalUniquenessConstraintDisplay.MultipleObjectifiedInternalConstraints;
		private static PreferredInternalUniquenessConstraintDisplay myCurrentPreferredInternalUniquenessConstraintDisplay = PreferredInternalUniquenessConstraintDisplay_Default;
		private PreferredInternalUniquenessConstraintDisplay myPreferredInternalUniquenessConstraintDisplay = PreferredInternalUniquenessConstraintDisplay_Default;
		#endregion // Member variables
		#region Base overrides
		/// <summary>
		/// Set the current values of the static properties
		/// to match the cached settings
		/// </summary>
		public override void LoadSettingsFromStorage()
		{
			base.LoadSettingsFromStorage();
			myCurrentObjectTypeDisplayShape = myObjectTypeDisplayShape;
			myCurrentObjectifiedFactDisplayShape = myObjectifiedFactDisplayShape;
			myCurrentMandatoryDotPlacement = myMandatoryDotPlacement;
			myCurrentRoleNameDisplay = myRoleNameDisplay;
			myCurrentDefaultDataType = myDefaultDataType;
			myCurrentExternalConstraintRoleBarDisplay = myExternalConstraintRoleBarDisplay;
			myCurrentPrimaryDeleteBehavior = myPrimaryDeleteBehavior;
			myCurrentFinalShapeDeleteBehavior = myFinalShapeDeleteBehavior;
			myCurrentCombineMandatoryAndUniqueVerbalization = myCombineMandatoryAndUniqueVerbalization;
			myCurrentShowDefaultConstraintVerbalization = myShowDefaultConstraintVerbalization;
			myCurrentCustomVerbalizationSnippets = myCustomVerbalizationSnippets;
			myCurrentPreferredInternalUniquenessConstraintDisplay = myPreferredInternalUniquenessConstraintDisplay;
		}
		/// <summary>
		/// Set local values for the current settings to determine later if the
		/// settings have changed in the OnApply method.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivate(CancelEventArgs e)
		{
			myObjectTypeDisplayShape = myCurrentObjectTypeDisplayShape;
			myObjectifiedFactDisplayShape = myCurrentObjectifiedFactDisplayShape;
			myMandatoryDotPlacement = myCurrentMandatoryDotPlacement;
			myRoleNameDisplay = myCurrentRoleNameDisplay;
			myDefaultDataType = myCurrentDefaultDataType;
			myExternalConstraintRoleBarDisplay = myCurrentExternalConstraintRoleBarDisplay;
			myPrimaryDeleteBehavior = myCurrentPrimaryDeleteBehavior;
			myFinalShapeDeleteBehavior = myCurrentFinalShapeDeleteBehavior;
			myCombineMandatoryAndUniqueVerbalization = myCurrentCombineMandatoryAndUniqueVerbalization;
			myShowDefaultConstraintVerbalization = myCurrentShowDefaultConstraintVerbalization;
			myCustomVerbalizationSnippets = myCurrentCustomVerbalizationSnippets;
			myPreferredInternalUniquenessConstraintDisplay = myCurrentPreferredInternalUniquenessConstraintDisplay;
		}

		/// <summary>
		/// Invalidate each loaded ORM diagram to force a redraw of the shapes
		/// </summary>
		/// <param name="e"></param>
		protected override void OnApply(DialogPage.PageApplyEventArgs e)
		{
			bool updateVerbalizer =
				myCurrentCombineMandatoryAndUniqueVerbalization != myCombineMandatoryAndUniqueVerbalization ||
				myCurrentShowDefaultConstraintVerbalization != myShowDefaultConstraintVerbalization ||
				myCurrentCustomVerbalizationSnippets != myCustomVerbalizationSnippets;
			// Get out early if none of the settings have changed
			if (myCurrentMandatoryDotPlacement == myMandatoryDotPlacement &&
				myCurrentObjectifiedFactDisplayShape == myObjectifiedFactDisplayShape &&
				myCurrentObjectTypeDisplayShape == myObjectTypeDisplayShape &&
				myCurrentRoleNameDisplay == myRoleNameDisplay &&
				myCurrentExternalConstraintRoleBarDisplay == myExternalConstraintRoleBarDisplay &&
				myCurrentPreferredInternalUniquenessConstraintDisplay == myPreferredInternalUniquenessConstraintDisplay)
			{
				// Non-displayed setting, don't notify
				myCurrentDefaultDataType = myDefaultDataType;
				myCurrentPrimaryDeleteBehavior = myPrimaryDeleteBehavior;
				myCurrentFinalShapeDeleteBehavior = myFinalShapeDeleteBehavior;
				myCurrentCombineMandatoryAndUniqueVerbalization = myCombineMandatoryAndUniqueVerbalization;
				myCurrentShowDefaultConstraintVerbalization = myShowDefaultConstraintVerbalization;
				myCurrentCustomVerbalizationSnippets = myCustomVerbalizationSnippets;
				if (updateVerbalizer)
				{
					ORMDesignerPackage.VerbalizationWindowSettingsChanged();
				}
				return;
			}

			// See if facts need resizing
			bool resizeFactShapes = myCurrentExternalConstraintRoleBarDisplay != myExternalConstraintRoleBarDisplay;
			bool updateRoleNames = myCurrentRoleNameDisplay != myRoleNameDisplay;

			// Set the new options
			myCurrentMandatoryDotPlacement = myMandatoryDotPlacement;
			myCurrentObjectifiedFactDisplayShape = myObjectifiedFactDisplayShape;
			myCurrentObjectTypeDisplayShape = myObjectTypeDisplayShape;
			myCurrentRoleNameDisplay = myRoleNameDisplay;
			myCurrentExternalConstraintRoleBarDisplay = myExternalConstraintRoleBarDisplay;
			myCurrentDefaultDataType = myDefaultDataType;
			myCurrentPrimaryDeleteBehavior = myPrimaryDeleteBehavior;
			myCurrentFinalShapeDeleteBehavior = myFinalShapeDeleteBehavior;
			myCurrentCombineMandatoryAndUniqueVerbalization = myCombineMandatoryAndUniqueVerbalization;
			myCurrentShowDefaultConstraintVerbalization = myShowDefaultConstraintVerbalization;
			myCurrentCustomVerbalizationSnippets = myCustomVerbalizationSnippets;
			myCurrentPreferredInternalUniquenessConstraintDisplay = myPreferredInternalUniquenessConstraintDisplay;

			// Walk all the documents and invalidate ORM diagrams if the options have changed
			NotifySettingsChange(
				Site,
				delegate(ORMDesignerDocData docData)
			{
				IList diagrams = docData.Store.ElementDirectory.GetElements(ORMDiagram.MetaClassGuid);
				int diagramCount = diagrams.Count;
				for (int i = 0; i < diagramCount; ++i)
				{
					ORMDiagram diagram = (ORMDiagram)diagrams[i];
					using (Transaction t = diagram.Store.TransactionManager.BeginTransaction(ResourceStrings.OptionsPageChangeTransactionName))
					{
						Store store = diagram.Store;
						foreach (BinaryLinkShape link in store.ElementDirectory.GetElements(BinaryLinkShape.MetaClassGuid, true))
						{
							link.RipUp();
						}
						if (resizeFactShapes)
						{
							foreach (FactTypeShape factShape in store.ElementDirectory.GetElements(FactTypeShape.MetaClassGuid, true))
							{
								factShape.AutoResize();
							}
						}
						if (updateRoleNames)
						{
							foreach (Role role in store.ElementDirectory.GetElements(Role.MetaClassGuid, false))
							{
								RoleNameShape.SetRoleNameDisplay(role.FactType);
							}
						}
						if (t.HasPendingChanges)
						{
							t.Commit();
						}
					}
					diagram.Invalidate(true);
				}
			});
			if (updateVerbalizer)
			{
				ORMDesignerPackage.VerbalizationWindowSettingsChanged();
			}
		}
		#endregion // Base overrides
		#region Change Notification Functions
		/// <summary>
		/// Walk all running ORMDesigner documents and callback to the
		/// notification delegate. Used to notify settings changes from
		/// the options page or other change sources (like the FontAndColors
		/// settings).
		/// </summary>
		/// <param name="serviceProvider">IServiceProvider</param>
		/// <param name="changeCallback">Delegate callback</param>
		public static void NotifySettingsChange(IServiceProvider serviceProvider, NotifyDocument changeCallback)
		{
			// Walk all the documents and invalidate ORM diagrams if the options have changed
			IVsRunningDocumentTable docTable = (IVsRunningDocumentTable)serviceProvider.GetService(typeof(IVsRunningDocumentTable));
			IEnumRunningDocuments docIter;
			ErrorHandler.ThrowOnFailure(docTable.GetRunningDocumentsEnum(out docIter));
			int hrIter;
			uint[] currentDocs = new uint[1];
			uint fetched = 0;
			do
			{
				ErrorHandler.ThrowOnFailure(hrIter = docIter.Next(1, currentDocs, out fetched));
				if (hrIter == 0)
				{
					uint grfRDTFlags;
					uint dwReadLocks;
					uint dwEditLocks;
					string bstrMkDocument;
					IVsHierarchy pHier;
					uint itemId;
					IntPtr punkDocData = IntPtr.Zero;
					ErrorHandler.ThrowOnFailure(docTable.GetDocumentInfo(
						currentDocs[0],
						out grfRDTFlags,
						out dwReadLocks,
						out dwEditLocks,
						out bstrMkDocument,
						out pHier,
						out itemId,
						out punkDocData));
					try
					{
						ORMDesignerDocData docData = Marshal.GetObjectForIUnknown(punkDocData) as ORMDesignerDocData;
						if (docData != null)
						{
							changeCallback(docData);
						}
					}
					finally
					{
						if (punkDocData != IntPtr.Zero)
						{
							Marshal.Release(punkDocData);
						}
					}
				}
			} while (fetched != 0);
		}
		#endregion Change Notification Functions
		#region Accessor properties
		/// <summary>
		/// Object Type Shape option
		/// </summary>
		[DefaultValue(ObjectTypeDisplayShape_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyObjectTypeShapeDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyObjectTypeShapeDisplayNameId)]
		public ObjectTypeDisplayShape ObjectTypeDisplayShape
		{
			get { return myObjectTypeDisplayShape; }
			set { myObjectTypeDisplayShape = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for ObjectTypeDisplayShape
		/// </summary>
		public static ObjectTypeDisplayShape CurrentObjectTypeDisplayShape
		{
			get { return myCurrentObjectTypeDisplayShape; }
		}

		/// <summary>
		/// Objectified Shape option
		/// </summary>
		[DefaultValue(ObjectifiedFactDisplayShape_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyObjectifiedShapeDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyObjectifiedShapeDisplayNameId)]
		public ObjectifiedFactDisplayShape ObjectifiedFactDisplayShape
		{
			get { return myObjectifiedFactDisplayShape; }
			set { myObjectifiedFactDisplayShape = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for ObjectifiedFactShape
		/// </summary>
		public static ObjectifiedFactDisplayShape CurrentObjectifiedFactDisplayShape
		{
			get { return myCurrentObjectifiedFactDisplayShape;  }
		}

		/// <summary>
		/// Objectified Shape option
		/// </summary>
		[DefaultValue(MandatoryDotPlacement_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyMandatoryDotDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyMandatoryDotDisplayNameId)]
		public MandatoryDotPlacement MandatoryDotPlacement
		{
			get { return myMandatoryDotPlacement; }
			set { myMandatoryDotPlacement = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for MandatoryDotPlacement
		/// </summary>
		public static MandatoryDotPlacement CurrentMandatoryDotPlacement
		{
			get { return myCurrentMandatoryDotPlacement; }
		}

		/// <summary>
		/// Display of role names
		/// </summary>
		[DefaultValue(RoleNameDisplay_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyRoleNameDisplayDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyRoleNameDisplayDisplayNameId)]
		public RoleNameDisplay RoleNameDisplay
		{
			get { return myRoleNameDisplay; }
			set { myRoleNameDisplay = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for RoleNameDisplay
		/// </summary>
		public static RoleNameDisplay CurrentRoleNameDisplay
		{
			get { return myCurrentRoleNameDisplay; }
		}

		/// <summary>
		/// Default data type for value types
		/// </summary>
		[DefaultValue(DefaultDataType_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryDataTypesId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyDataTypeDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyDataTypeDisplayNameId)]
		public PortableDataType DefaultDataType
		{
			get { return myDefaultDataType; }
			set
			{
				// UNDONE: Figure out how to get this value out of the list
				if (value != PortableDataType.UserDefined)
				{
					myDefaultDataType = value;
				}
			}
		}

		/// <summary>
		/// Current VS session-wide setting default data type
		/// when a new value type is added
		/// </summary>
		public static PortableDataType CurrentDefaultDataType
		{
			get { return myCurrentDefaultDataType; }
		}

		/// <summary>
		/// Display of external constraint bars
		/// </summary>
		[DefaultValue(ExternalConstraintRoleBarDisplay_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyExternalConstraintRoleBarDisplayDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyExternalConstraintRoleBarDisplayNameId)]
		public ExternalConstraintRoleBarDisplay ExternalConstraintRoleBarDisplay
		{
			get { return myExternalConstraintRoleBarDisplay; }
			set { myExternalConstraintRoleBarDisplay = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for ExternalConstraintRoleBarDisplay
		/// </summary>
		public static ExternalConstraintRoleBarDisplay CurrentExternalConstraintRoleBarDisplay
		{
			get { return myCurrentExternalConstraintRoleBarDisplay; }
		}

		/// <summary>
		/// Behavior of Delete and Control-Delete keys
		/// </summary>
		[DefaultValue(PrimaryDeleteBehavior_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryDeleteBehaviorId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyPrimaryDeleteBehaviorDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyPrimaryDeleteBehaviorDisplayNameId)]
		public PrimaryDeleteBehavior PrimaryDeleteBehavior
		{
			get { return myPrimaryDeleteBehavior; }
			set { myPrimaryDeleteBehavior = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for PrimaryDeleteBehavior
		/// </summary>
		public static PrimaryDeleteBehavior CurrentPrimaryDeleteBehavior
		{
			get { return myCurrentPrimaryDeleteBehavior; }
		}

		/// <summary>
		/// Behavior of final shape deletion
		/// </summary>
		[DefaultValue(FinalShapeDeleteBehavior_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryDeleteBehaviorId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyFinalShapeDeleteBehaviorDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyFinalShapeDeleteBehaviorDisplayNameId)]
		public FinalShapeDeleteBehavior FinalShapeDeleteBehavior
		{
			get { return myFinalShapeDeleteBehavior; }
			set { myFinalShapeDeleteBehavior = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for PrimaryDeleteBehavior
		/// </summary>
		public static FinalShapeDeleteBehavior CurrentFinalShapeDeleteBehavior
		{
			get { return myCurrentFinalShapeDeleteBehavior; }
		}

		/// <summary>
		/// Current setting for CombineMandatoryAndUniqueVerbalization
		/// </summary>
		[DefaultValue(CombineMandatoryAndUniqueVerbalization_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryVerbalizationBehaviorId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyCombineMandatoryAndUniqueVerbalizationDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyCombineMandatoryAndUniqueVerbalizationDisplayNameId)]
		public bool CombineMandatoryAndUniqueVerbalization
		{
			get { return myCombineMandatoryAndUniqueVerbalization; }
			set { myCombineMandatoryAndUniqueVerbalization = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for CombineMandatoryAndUniqueVerbalization
		/// </summary>
		public static bool CurrentCombineMandatoryAndUniqueVerbalization
		{
			get { return myCurrentCombineMandatoryAndUniqueVerbalization; }
		}

		/// <summary>
		/// Current setting for ShowDefaultConstraintVerbalization
		/// </summary>
		[DefaultValue(ShowDefaultConstraintVerbalization_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryVerbalizationBehaviorId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyShowDefaultConstraintVerbalizationDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyShowDefaultConstraintVerbalizationDisplayNameId)]
		public bool ShowDefaultConstraintVerbalization
		{
			get { return myShowDefaultConstraintVerbalization; }
			set { myShowDefaultConstraintVerbalization = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for ShowDefaultConstraintVerbalization
		/// </summary>
		public static bool CurrentShowDefaultConstraintVerbalization
		{
			get { return myCurrentShowDefaultConstraintVerbalization; }
		}

		/// <summary>
		/// Current setting for CustomVerbalizationSnippets
		/// </summary>
		[DefaultValue(CustomVerbalizationSnippets_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryVerbalizationBehaviorId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyCustomVerbalizationSnippetsDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyCustomVerbalizationSnippetsDisplayNameId)]
		public string CustomVerbalizationSnippets
		{
			get { return myCustomVerbalizationSnippets; }
			set { myCustomVerbalizationSnippets = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for CustomVerbalizationSnippets
		/// </summary>
		public static string CurrentCustomVerbalizationSnippets
		{
			get { return myCurrentCustomVerbalizationSnippets; }
		}

		/// <summary>
		/// Current setting for PreferredInternalUniquenessConstraintDisplay
		/// </summary>
		[DefaultValue(PreferredInternalUniquenessConstraintDisplay_Default)]
		[LocalizedCategory(ResourceStrings.OptionsPageCategoryAppearanceId)]
		[LocalizedDescription(ResourceStrings.OptionsPagePropertyPreferredInternalUniquenessConstraintDisplayDescriptionId)]
		[LocalizedDisplayName(ResourceStrings.OptionsPagePropertyPreferredInternalUniquenessConstraintDisplayDisplayNameId)]
		public PreferredInternalUniquenessConstraintDisplay PreferredInternalUniquenessConstraintDisplay
		{
			get { return myPreferredInternalUniquenessConstraintDisplay; }
			set { myPreferredInternalUniquenessConstraintDisplay = value; }
		}

		/// <summary>
		/// Current VS session-wide setting for PreferredInternalUniquenessConstraintDisplay
		/// </summary>
		public static PreferredInternalUniquenessConstraintDisplay CurrentPreferredInternalUniquenessConstraintDisplay
		{
			get { return myCurrentPreferredInternalUniquenessConstraintDisplay; }
		}
		#endregion // Accessor properties
	}
}
