-- ================================================================
-- ZERA O BANCO: REMOVE SCHEMAS ANTERIORES
-- ================================================================
DROP SCHEMA IF EXISTS contacorrente CASCADE;
DROP SCHEMA IF EXISTS transferencia CASCADE;
DROP SCHEMA IF EXISTS tarifa CASCADE;

-- ================================================================
-- SCHEMAS
-- ================================================================
CREATE SCHEMA contacorrente;
CREATE SCHEMA transferencia;
CREATE SCHEMA tarifa;

-- ================================================================
-- CONTA CORRENTE
-- ================================================================
CREATE TABLE contacorrente.contacorrente (
	idcontacorrente VARCHAR(37) PRIMARY KEY, -- id da conta corrente
	numero INTEGER NOT NULL UNIQUE, -- numero da conta corrente
	nome VARCHAR(100) NOT NULL, -- nome do titular da conta corrente
	ativo INTEGER NOT NULL default 0, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
	senha VARCHAR(100) NOT NULL,
	salt VARCHAR(100) NOT NULL,
	CHECK (ativo in (0,1))
);

CREATE TABLE contacorrente.movimento (
	idmovimento VARCHAR(37) PRIMARY KEY, -- identificacao unica do movimento
	idcontacorrente VARCHAR(37) NOT NULL, -- identificacao unica da conta corrente
	datamovimento VARCHAR(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
	tipomovimento VARCHAR(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
	valor REAL NOT NULL, -- valor do movimento. Usar duas casas decimais.
	CHECK (tipomovimento in ('C','D')),
	FOREIGN KEY(idcontacorrente) REFERENCES contacorrente.contacorrente(idcontacorrente)
);

CREATE TABLE contacorrente.idempotencia (
	chave_idempotencia VARCHAR(37) PRIMARY KEY, -- identificacao chave de idempotencia
	requisicao VARCHAR(1000), -- dados de requisicao
	resultado VARCHAR(1000) -- dados de retorno
);

-- ================================================================
-- TRANSFERENCIA
-- ================================================================
CREATE TABLE transferencia.transferencia (
	idtransferencia VARCHAR(37) PRIMARY KEY, -- identificacao unica da transferencia
	idcontacorrente_origem VARCHAR(37) NOT NULL, -- identificacao unica da conta corrente de origem
	idcontacorrente_destino VARCHAR(37) NOT NULL, -- identificacao unica da conta corrente de destino
	datamovimento VARCHAR(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor NUMERIC(15,2) NOT NULL, -- valor da transferencia. Usar duas casas decimais.
	FOREIGN KEY(idtransferencia) REFERENCES transferencia.transferencia(idtransferencia)
);

CREATE TABLE transferencia.idempotencia (
	chave_idempotencia VARCHAR(37) PRIMARY KEY, -- identificacao chave de idempotencia
	requisicao VARCHAR(1000), -- dados de requisicao
	resultado VARCHAR(1000) -- dados de retorno
);

-- ================================================================
-- TARIFA
-- ================================================================
CREATE TABLE tarifa.tarifa (
	idtarifa VARCHAR(37) PRIMARY KEY, -- identificacao unica da tarifa
	idcontacorrente VARCHAR(37) NOT NULL, -- identificacao unica da conta corrente
	datamovimento VARCHAR(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor NUMERIC(15,2) NOT NULL, -- valor da tarifa. Usar duas casas decimais.
	FOREIGN KEY(idtarifa) REFERENCES tarifa.tarifa(idtarifa)
);