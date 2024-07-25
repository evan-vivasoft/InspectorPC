/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand41List
    /// </summary>
    public class ScriptCommand41List
    {
        /// <summary>
        /// Gets or sets the sequence number list.
        /// </summary>
        /// <value>The sequence number list.</value>
        public long SequenceNumberList { get; set; }

        /// <summary>
        /// Gets or sets the type of the list.
        /// </summary>
        /// <value>The type of the list.</value>
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selection required].
        /// </summary>
        /// <value><c>true</c> if [selection required]; otherwise, <c>false</c>.</value>
        public bool SelectionRequired { get; set; }

        /// <summary>
        /// Gets or sets the list question.
        /// </summary>
        /// <value>The list question.</value>
        public string ListQuestion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [one selection allowed].
        /// </summary>
        /// <value><c>true</c> if [one selection allowed]; otherwise, <c>false</c>.</value>
        public bool OneSelectionAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check list result].
        /// </summary>
        /// <value><c>true</c> if [check list result]; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CheckList")]
        public bool CheckListResult { get; set; }

        /// <summary>
        /// Gets or sets the list condition codes.
        /// </summary>
        /// <value>The list condition codes.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ListConditionCode> ListConditionCodes { get; set; }
    }

    /// <summary>
    /// ListConditionCode
    /// </summary>
    public class ListConditionCode
    {
        /// <summary>
        /// Gets or sets the condition code.
        /// </summary>
        /// <value>The condition code.</value>
        public string ConditionCode { get; set; }

        /// <summary>
        /// Gets or sets the condition code description.
        /// </summary>
        /// <value>The condition code description.</value>
        public string ConditionCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [display next list].
        /// </summary>
        /// <value><c>true</c> if [display next list]; otherwise, <c>false</c>.</value>
        public bool DisplayNextList { get; set; }
    }
}
