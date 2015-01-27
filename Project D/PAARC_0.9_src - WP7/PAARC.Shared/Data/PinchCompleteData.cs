﻿
using System.IO;

namespace PAARC.Shared.Data
{
    /// <summary>
    /// A data message that represents a completed pinch gesture.
    /// </summary>
    public sealed class PinchCompleteData : DataMessage
    {
        #region Overrides of DataMessage

        /// <summary>
        /// Returns data type <c>PinchComplete</c>.
        /// </summary>
        public override DataType DataType
        {
            get
            {
                return DataType.PinchComplete;
            }
        }

        /// <summary>
        /// Returns <c>true</c>.
        /// </summary>
        public override bool MustBeDelivered
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Adds custom data to the raw representation of the data message.
        /// </summary>
        /// <param name="writer">The binary writer used to create the raw representation of the data message.</param>
        protected override void WriteData(BinaryWriter writer)
        {
        }

        /// <summary>
        /// Reads back custom data from a raw representation of the data message.
        /// </summary>
        /// <param name="reader">The binary reader used to read from the raw representation of the original data message.</param>
        protected override void ReadData(BinaryReader reader)
        {
        }

        #endregion
    }
}
