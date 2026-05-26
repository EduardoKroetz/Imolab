using Imolab.Domain;

var proprietarioId = Guid.NewGuid();
var inquilinoId = Guid.NewGuid();
var responsavelImobiliariaId = Guid.NewGuid();
var valorAluguel = 1500m;

var contrato = new ContratoLocacao(
    imovelId: Guid.NewGuid(),
    proprietarioId,
    inquilinoId,
    valorAluguel
);

var vistoria = new Vistoria(
    contratoLocacaoId: contrato.Id,
    descricao: "Vistoria inicial do imóvel.",
    tipo: TipoVistoria.VistoriaEntrada
);

contrato.RegistrarVistoria(vistoria);

contrato.AssinarContrato(TipoParte.Proprietario);
contrato.AssinarContrato(TipoParte.Inquilino);
contrato.AssinarContrato(TipoParte.Imobiliaria, responsavelImobiliariaId);

var pagamentoAluguel = new PagamentoContrato(
    contratoLocacaoId: contrato.Id,
    tipo: TipoPagamento.Aluguel,
    valor: valorAluguel
);

contrato.EntregarChavesImovel();

contrato.RegistrarPagamento(pagamentoAluguel);

contrato.EncerrarContrato();
