namespace ReklamationAPI.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Custom implementation of annotation for validation of complaint status value.
    /// </summary>
    public class ValidComplaintStatus : RequiredAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return Enum.GetNames(typeof(ComplaintStatus)).Contains(value.ToString());
        }
    }
}
