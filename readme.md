# Introdução
Esse app foi feito para analisar o estado e manter um histórico de estados de um Nobreak da TS Shara à partir da porta Serial. Mais especificamente o modelo "UPS Senoidal Universal 2200VA". Porém imagino que funcione com outros também.

# Tecnologias
Foi usado ASP.NET Core juntamente com o Entity Framework Core.
O banco de dados configurado nas migrações incluídas foi o MySQL

# Configurações
As configurações são definidas no appsettings.
As que requerem atenção:
## `Variables:SerialPort`
Essa variável armazenará o nome da porta serial que o app usará para se comunicar com o nobreak. Se não definida, ou vazia, será usada a última porta da lista retornada pelo método `SerialPort.GetPortNames()`.
  
  
# [Exemplo em produção](https://camilla.thiagofnsc.dev)