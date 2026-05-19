using PROJECTKPL.API.Models;
using FluentValidation;
using FluentValidation.Results;

namespace PROJECTKPL.API.Validators
{
    public class PelangganValidator : AbstractValidator<Pelanggan>
    {
        public PelangganValidator()
        {
            RuleSet("Username", () =>
            {
                RuleFor(p => p.Username)
                    .NotEmpty().WithMessage("Username harus diisi")
                    .MinimumLength(3).WithMessage("Username minimal 3 karakter")
                    .MaximumLength(50).WithMessage("Username maksimal 50 karakter");
            });

            RuleSet("Gender", () =>
            {
                RuleFor(p => p.Gender)
                    .NotEmpty().WithMessage("Gender harus diisi")
                    .Must(g => g == "Laki-laki" || g == "Perempuan")
                    .WithMessage("Gender harus 'Laki-laki' atau 'Perempuan'");
            });

            RuleSet("NoTelp", () =>
            {
                RuleFor(p => p.NoTelp)
                    .NotEmpty().WithMessage("No. telepon harus diisi")
                    .Matches(@"^08[0-9]{8,11}$").WithMessage("No. telepon harus diawali 08 dan terdiri dari 10-13 digit");
            });

            RuleSet("Umur", () =>
            {
                RuleFor(p => p.Umur)
                    .InclusiveBetween(1, 120).WithMessage("Umur harus antara 1 sampai 120");
            });

            RuleSet("Password", () =>
            {
                RuleFor(p => p.Password)
                    .NotEmpty().WithMessage("Password harus diisi")
                    .MinimumLength(6).WithMessage("Password minimal 6 karakter")
                    .Matches("[A-Z]").WithMessage("Password harus memiliki minimal 1 huruf kapital")
                    .Matches("[0-9]").WithMessage("Password harus memiliki minimal 1 angka");
            });
        }

        // Validasi semua ruleset sekaligus
        public ValidationResult ValidateAll(Pelanggan pelanggan)
        {
            return base.Validate(new ValidationContext<Pelanggan>(
                pelanggan, null,
                new FluentValidation.Internal.RulesetValidatorSelector(
                    new[] { "Username", "Gender", "NoTelp", "Umur", "Password" }
                )
            ));
        }

        // Validasi ruleset tertentu saja
        public ValidationResult Validate(Pelanggan pelanggan, string ruleSet)
        {
            return base.Validate(new ValidationContext<Pelanggan>(
                pelanggan, null,
                new FluentValidation.Internal.RulesetValidatorSelector(ruleSet.Split(','))
            ));
        }
    }
}
