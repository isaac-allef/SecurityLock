#TO DO

- 3 tipos de engine
    - Par de chaves: 
        (Ok) limite de combinação A com B, 
        (Ok) limite de combinação B com A, 
        () block list de combinação
    - Uma chave: 
        (Ok) rate limit, 
        (Ok) block list, 
        () allow list, 
        () regex
    - Sem chave: 
        () bloqueio simples(true or false), 
        () bloqueio por agenda (por horário, por dia, por data)

- fazer o simples e útil
    - Par de chaves:
        (Ok) limite de combinação A com B, 
        (Ok) limite de combinação B com A, 
        (Ok) block list de A
        (Ok) block list de B
- outro produto
    - Toggle
        () bloqueio por agenda (por horário, por dia, por data)

- Em outro projeto fazer um pacote que tenha uma engine que receba um objeto e que o usuário crie validações para eles, ou seja, uma cascata de validação flexivel.
Essa cascata receberá um input(string) e extras(dictionary)