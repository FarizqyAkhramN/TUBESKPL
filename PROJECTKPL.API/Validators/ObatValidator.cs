using PROJECTKPL.API.Models;
using FluentValidation;
using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace PROJECTKPL.API.Validators
{
    public class ObatValidator : AbstractValidator<Obat>
    {
        public ObatValidator()
        {
            RuleSet("NamaObat", () =>
            {
                RuleFor(o => o.NamaObat)
                    .NotEmpty().WithMessage("Nama obat harus diisi")
                    .MinimumLength(3).WithMessage("Nama obat minimal 3 karakter")
                    .MaximumLength(100).WithMessage("Nama obat maksimal 100 karakter");
            });

            RuleSet("Stok", () =>
            {
                RuleFor(o => o.Stok)
                    .GreaterThanOrEqualTo(0).WithMessage("Stok tidak boleh negatif");
            });

            RuleSet("Harga", () =>
            {
                RuleFor(o => o.Harga)
                    .GreaterThan(0).WithMessage("Harga harus lebih dari 0");
            });
        }

        // Validasi semua ruleset sekaligus
        public ValidationResult ValidateAll(Obat obat)
        {
            return base.Validate(new ValidationContext<Obat>(
                obat, null,
                new FluentValidation.Internal.RulesetValidatorSelector(
                    new[] { "NamaObat", "Stok", "Harga" }
                )
            ));
        }

        // Validasi ruleset tertentu saja
        public ValidationResult Validate(Obat obat, string ruleSet)
        {
            return base.Validate(new ValidationContext<Obat>(
                obat, null,
                new FluentValidation.Internal.RulesetValidatorSelector(ruleSet.Split(','))
            ));
        }
    }

}
