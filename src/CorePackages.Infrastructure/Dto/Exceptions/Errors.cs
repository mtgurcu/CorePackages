namespace CorePackages.Infrastructure.Dto.Exceptions
{
    public readonly struct Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
    public static class Errors
    {
        public static class Patient
        {
            public static readonly Error PatientExists = new("P001", "Hasta sistemde kayıtlı.");
            public static readonly Error PatientNotExist = new("P002", "Hasta kayıtlı değil.");
            public static readonly Error TreatmentPackageNotExist = new("P003", "Tedavi paketi kayıtlı değil.");
            public static readonly Error ReservationExist = new("P004", "Reservasyon zaten mevcut.");
            public static readonly Error ReservationNotExist = new("P005", "Reservasyon mevcut değil.");
            public static readonly Error PatientCreateError = new("P006", "Hasta kayıt edilemedi.");
            public static readonly Error PatientUpdateError = new("P007", "Hasta güncellenemedi.");
        }
        public static class Clinic
        {
            public static readonly Error ClinicExists = new("C001", "Klinik sistemde kayıtlı.");
            public static readonly Error ClinicNotExists = new("C002", "Klinik sistemde kayıtlı değil.");
        }
        public static class Outbox
        {
            public static Error OutboxExists(string typeName, string key) => new Error("O001", $"{typeName} type'lı kayıt {key} key ile mevcut.");
        }
        public static class Validation
        {
            public static Error ValidationErrors(string errors) => new Error("V001", $"{errors}");
        }
    }
}
