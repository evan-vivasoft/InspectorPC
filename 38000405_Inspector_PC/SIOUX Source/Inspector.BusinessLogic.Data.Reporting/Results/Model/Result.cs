/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class Result
    {
        private const string TIMESPAN_FORMAT = "HH:mm:ss";

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the object ID.
        /// </summary>
        /// <value>
        /// The object ID.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string ObjectID { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>
        /// The measure point.
        /// </value>
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the measure point ID.
        /// </summary>
        /// <value>
        /// The measure point ID.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string MeasurePointID { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>
        /// The field no.
        /// </value>
        public int? FieldNo { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the measure value.
        /// </summary>
        /// <value>
        /// The measure value.
        /// </value>
        public MeasureValue MeasureValue { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlElement]
        public List<string> List { get; set; }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        [XmlIgnore]
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the object name description.
        /// </summary>
        /// <value>
        /// The object name description.
        /// </value>
        [XmlIgnore]
        public string ObjectNameDescription { get; set; }

        /// <summary>
        /// Gets or sets the measure point description.
        /// </summary>
        /// <value>
        /// The measure point description.
        /// </value>
        [XmlIgnore]
        public string MeasurePointDescription { get; set; }

        /// <summary>
        /// Shoulds the serialize field no.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeFieldNo()
        {
            return FieldNo.HasValue;
        }


        /// <summary>
        /// Prevents a default instance of the <see cref="Result"/> class from being created.
        /// </summary>
        private Result()
        {
            ObjectName = string.Empty;
            ObjectID = string.Empty;
            MeasurePoint = string.Empty;
            MeasurePointID = string.Empty;
            Time = string.Empty;
            SequenceNumber = -1;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Result"/> class from being created.
        /// Required for XML Serialization
        /// </summary>
        public Result(TimeSpan time)
            : this()
        {
            DateTime dateTimeFromTimeSpan = new DateTime(time.Ticks);
            Time = dateTimeFromTimeSpan.ToString(TIMESPAN_FORMAT, CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="measurePointId">The measure point id.</param>
        /// <param name="time">The time.</param>
        public Result(long sequenceNumber, string objectName, string objectId, string measurePoint, string measurePointId, TimeSpan time)
            : this(time)
        {
            ObjectName = objectName;
            ObjectID = objectId;
            MeasurePoint = measurePoint;
            MeasurePointID = measurePointId;
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="measurePointId">The measure point id.</param>
        /// <param name="fieldNo">The field no.</param>
        /// <param name="time">The time.</param>
        /// <param name="measureValue">The measure value.</param>
        /// <param name="text">The text.</param>
        /// <param name="list">The list.</param>
        /// <param name="objectNameDescription"></param>
        /// <param name="measurePointDescription"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public Result(long sequenceNumber, string objectName, string objectId, string measurePoint, string measurePointId, int? fieldNo, TimeSpan time, MeasureValue measureValue, string text, List<string> list, string objectNameDescription, string measurePointDescription)
            : this(sequenceNumber, objectName, objectId, measurePoint, measurePointId, time)
        {
            FieldNo = fieldNo;
            MeasureValue = measureValue;
            Text = text;
            List = list;
            MeasurePointDescription = measurePointDescription;
            ObjectNameDescription = objectNameDescription;
        }

        public void SetText(string text)
        {
            Text = text;
            Time = DateTime.Now.ToString(TIMESPAN_FORMAT, CultureInfo.InvariantCulture);
        }
    }
}
