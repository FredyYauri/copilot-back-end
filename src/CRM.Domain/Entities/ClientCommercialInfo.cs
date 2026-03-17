namespace CRM.Domain.Entities;

public class ClientCommercialInfo
{
    public Guid ClientId { get; private set; }
    public string? AsesorComercial { get; private set; }
    public string? CodigoAsesor { get; private set; }
    public string? MedioCaptacion { get; private set; }
    public string? CentralRiesgo { get; private set; }
    public decimal? LineaCredito { get; private set; }
    public string? Comentarios { get; private set; }

    private ClientCommercialInfo() { }

    public static ClientCommercialInfo Create(
        Guid clientId,
        string? asesorComercial,
        string? codigoAsesor,
        string? medioCaptacion,
        string? centralRiesgo,
        decimal? lineaCredito,
        string? comentarios)
    {
        return new ClientCommercialInfo
        {
            ClientId = clientId,
            AsesorComercial = asesorComercial,
            CodigoAsesor = codigoAsesor,
            MedioCaptacion = medioCaptacion,
            CentralRiesgo = centralRiesgo,
            LineaCredito = lineaCredito,
            Comentarios = comentarios
        };
    }

    public void Update(
        string? asesorComercial,
        string? codigoAsesor,
        string? medioCaptacion,
        string? centralRiesgo,
        decimal? lineaCredito,
        string? comentarios)
    {
        AsesorComercial = asesorComercial;
        CodigoAsesor = codigoAsesor;
        MedioCaptacion = medioCaptacion;
        CentralRiesgo = centralRiesgo;
        LineaCredito = lineaCredito;
        Comentarios = comentarios;
    }
}
