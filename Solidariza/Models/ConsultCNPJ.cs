
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class ApicnpjConsultResponseObject
    {
        public bool IsSuccess { get; set; }

        public string? Response {  get; set; }
    }

    public class ApicnpjConsultResponseError
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("detalhes")]
        public string? Detalhes { get; set; }
    }

    public class ApicnpjConsultResponse
    {
        [JsonPropertyName("cnpj_raiz")]
        public string? CnpjRaiz { get; set; }

        [JsonPropertyName("razao_social")]
        public string? RazaoSocial { get; set; }
        
        [JsonPropertyName("capital_social")]        
        public string? CapitalSocial { get; set; }

        [JsonPropertyName("responsavel_federativo")]
        public string? ResponsavelFederativo { get; set; }

        [JsonPropertyName("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }

        [JsonPropertyName("porte")]
        public Porte? Porte { get; set; }

        [JsonPropertyName("natureza_juridica")]
        public NaturezaJuridica? NaturezaJuridica { get; set; }

        [JsonPropertyName("qualificacao_do_responsavel")]
        public Qualificacao? QualificacaoDoResponsavel { get; set; }

        [JsonPropertyName("socios")]
        public List<Socio>? Socios { get; set; }

        [JsonPropertyName("simples")]
        public object? Simples { get; set; }

        [JsonPropertyName("estabelecimento")]
        public Estabelecimento? Estabelecimento { get; set; }
    }

    public class Porte
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("descrição")]
        public string? Descricao { get; set; }
    }

    public class NaturezaJuridica
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class Qualificacao
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class Socio
    {
        [JsonPropertyName("cpf_cnpj_socio")]
        public string? CpfCnpjSocio { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }

        [JsonPropertyName("data_entrada")]
        public DateTime? DataEntrada { get; set; }

        [JsonPropertyName("cpf_representante_legal")]
        public string? CpfRepresentanteLegal { get; set; }

        [JsonPropertyName("nome_representante")]
        public string? NomeRepresentante { get; set; }

        [JsonPropertyName("faixa_etaria")]
        public string? FaixaEtaria { get; set; }

        [JsonPropertyName("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }

        [JsonPropertyName("pais_id")]
        public string? PaisId { get; set; }

        [JsonPropertyName("qualificacao_socio")]
        public Qualificacao? QualificacaoSocio { get; set; }

        [JsonPropertyName("qualificacao_representante")]
        public Qualificacao? QualificacaoRepresentante { get; set; }

        [JsonPropertyName("pais")]
        public Pais? Pais { get; set; }
    }

    public class Pais
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("iso2")]
        public string? Iso2 { get; set; }

        [JsonPropertyName("iso3")]
        public string? Iso3 { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("comex_id")]
        public string? ComexId { get; set; }
    }

    public class Estabelecimento
    {
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        [JsonPropertyName("atividades_secundarias")]
        public List<AtividadeSecundaria>? AtividadesSecundarias { get; set; }

        [JsonPropertyName("cnpj_raiz")]
        public string? CnpjRaiz { get; set; }

        [JsonPropertyName("cnpj_ordem")]
        public string? CnpjOrdem { get; set; }

        [JsonPropertyName("cnpj_digito_verificador")]
        public string? CnpjDigitoVerificador { get; set; }

        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }

        [JsonPropertyName("nome_fantasia")]
        public string? NomeFantasia { get; set; }

        [JsonPropertyName("situacao_cadastral")]
        public string? SituacaoCadastral { get; set; }

        [JsonPropertyName("data_situacao_cadastral")]
        public DateTime? DataSituacaoCadastral { get; set; }

        [JsonPropertyName("data_inicio_atividade")]
        public DateTime? DataInicioAtividade { get; set; }

        [JsonPropertyName("nome_cidade_exterior")]
        public string? NomeCidadeExterior { get; set; }

        [JsonPropertyName("tipo_logradouro")]
        public string? TipoLogradouro { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("numero")]
        public string? Numero { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("ddd1")]
        public string? Ddd1 { get; set; }

        [JsonPropertyName("telefone1")]
        public string? Telefone1 { get; set; }

        [JsonPropertyName("ddd2")]
        public string? Ddd2 { get; set; }

        [JsonPropertyName("telefone2")]
        public string? Telefone2 { get; set; }

        [JsonPropertyName("ddd_fax")]
        public string? DddFax { get; set; }

        [JsonPropertyName("fax")]
        public string? Fax { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("situacao_especial")]
        public string? SituacaoEspecial { get; set; }

        [JsonPropertyName("data_situacao_especial")]
        public DateTime? DataSituacaoEspecial { get; set; }

        [JsonPropertyName("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }

        [JsonPropertyName("atividade_principal")]
        public AtividadePrincipal? AtividadePrincipal { get; set; }

        [JsonPropertyName("pais")]
        public Pais? Pais { get; set; }

        [JsonPropertyName("estador")]
        public Estado? Estado { get; set; }

        [JsonPropertyName("cidade")]
        public Cidade? Cidade { get; set; }

        [JsonPropertyName("motivo_situacao_cadastral")]
        public MotivoSituacaoCadastral? MotivoSituacaoCadastral { get; set; }

        [JsonPropertyName("inscricoes_estaduais")]
        public List<object>? InscricoesEstaduais { get; set; }
    }

    public class MotivoSituacaoCadastral
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class AtividadeSecundaria
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("secao")]
        public string? Secao { get; set; }

        [JsonPropertyName("divisao")]
        public string? Divisao { get; set; }

        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        [JsonPropertyName("classe")]
        public string? Classe { get; set; }

        [JsonPropertyName("subclasee")]
        public string? Subclasse { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class AtividadePrincipal
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("secao")]
        public string? Secao { get; set; }

        [JsonPropertyName("divisao")]
        public string? Divisao { get; set; }

        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        [JsonPropertyName("classe")]
        public string? Classe { get; set; }

        [JsonPropertyName("subclasse")]
        public string? Subclasse { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
    }

    public class Estado
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("sigla")]
        public string? Sigla { get; set; }

        [JsonPropertyName("ibge_id")]
        public int? IbgeId { get; set; }
    }

    public class Cidade
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("ibge_id")]
        public int? IbgeId { get; set; }

        [JsonPropertyName("siafi_id")]
        public string? SiafiId { get; set; }
    }

    public class ConsultCnpjResponse
    {
        public bool IsValid { get; set; }

        public string? DisapprovalReason { get; set; } 
    }
}
