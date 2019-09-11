using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses.Helpers
{
    public static class ValidationHelper
    {
        private static bool debugErrors = true;
        public static List<ModelValidationError> ValidateModel(CModel model)
        {
            List<ModelValidationError> errors = new List<ModelValidationError>();

            errors.AddRange(ValidateMembersCrscWidth(model));

            if (debugErrors) WriteErrors(errors);
            return errors;
        }

        private static List<ModelValidationError> ValidateMembersCrscWidth(CModel model)
        {
            List<ModelValidationError> errors = new List<ModelValidationError>();

            double crscWidth = double.NaN;
            foreach (CMember m in model.m_arrMembers)
            {
                if (m.EMemberTypePosition == EMemberType_FS_Position.MainColumn || m.EMemberTypePosition == EMemberType_FS_Position.MainRafter)
                {
                    if (m.CrScStart == null && m.CrScEnd == null) continue;
                    double crscStartB = m.CrScStart != null ? m.CrScStart.b : double.NaN;
                    double crscEndB = m.CrScEnd != null ? m.CrScEnd.b : double.NaN;
                    if (crscWidth.Equals(double.NaN) && !crscStartB.Equals(double.NaN)) { crscWidth = crscStartB; continue; }
                    if (crscWidth.Equals(double.NaN) && !crscEndB.Equals(double.NaN)) { crscWidth = crscEndB; continue; }

                    if (!crscStartB.Equals(double.NaN))
                    {
                        if (!MathF.d_equal(crscWidth, crscStartB, crscWidth / 100 * 5))
                        { errors.Add(new ModelValidationError(1, "CRSC widths", $"Main Rafter and Main Column have different crsc widths.")); break; }
                    }
                    if (!crscEndB.Equals(double.NaN))
                    {
                        if (!MathF.d_equal(crscWidth, crscEndB, crscWidth / 100 * 5))
                        { errors.Add(new ModelValidationError(1, "CRSC widths", $"Main Rafter and Main Column have different crsc widths.")); break; }
                    }
                }
            }

            crscWidth = double.NaN;
            foreach (CMember m in model.m_arrMembers)
            {
                if (m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn || m.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                {
                    if (m.CrScStart == null && m.CrScEnd == null) continue;
                    double crscStartB = m.CrScStart != null ? m.CrScStart.b : double.NaN;
                    double crscEndB = m.CrScEnd != null ? m.CrScEnd.b : double.NaN;
                    if (crscWidth.Equals(double.NaN) && !crscStartB.Equals(double.NaN)) { crscWidth = crscStartB; continue; }
                    if (crscWidth.Equals(double.NaN) && !crscEndB.Equals(double.NaN)) { crscWidth = crscEndB; continue; }

                    if (!crscStartB.Equals(double.NaN))
                    {
                        if (!MathF.d_equal(crscWidth, crscStartB, crscWidth / 100 * 5))
                        { errors.Add(new ModelValidationError(1, "CRSC widths", $"Edge Rafter and Edge Column have different crsc widths.")); break; }
                    }
                    if (!crscEndB.Equals(double.NaN))
                    {
                        if (!MathF.d_equal(crscWidth, crscEndB, crscWidth / 100 * 5))
                        { errors.Add(new ModelValidationError(1, "CRSC widths", $"Edge Rafter and Edge Column have different crsc widths.")); break; }
                    }
                }
            }

            return errors;
        }

        private static void WriteErrors(List<ModelValidationError> errors)
        {
            foreach (ModelValidationError error in errors)
            {
                System.Diagnostics.Trace.WriteLine($"[{error.ID}]. {error.Msg}\t{error.Description}");
            }

        }

        public static string GetErrorsString(List<ModelValidationError> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ModelValidationError error in errors)
            {
                sb.AppendLine($"[{error.ID}]. {error.Msg}\t{error.Description}");
            }
            return sb.ToString();
        }
    }
}
