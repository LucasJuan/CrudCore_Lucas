CREATE TABLE acessa (
id INT  primary key AUTO_INCREMENT NOT NULL,
nome VARCHAR(255) null,
senha VARCHAR(255)  null,
email VARCHAR(255)  null,
data_inclusao DATETIME DEFAULT CURRENT_TIMESTAMP,
data_alteracao DATETIME ON UPDATE CURRENT_TIMESTAMP,
data_exclusao DATETIME,
chave VARCHAR(255),
ativo CHAR(1),
role VARCHAR(255)
)
