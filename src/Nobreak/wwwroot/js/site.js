function loadTo(url, element) {
    fetch(url)
        .then(res => {
            if (res.ok)
                return res.text();
            else
                $(element).html('<div class="alert alert-danger">Erro buscando o conteúdo :/</div>');
        })
        .then(body =>
            $(element).html(body));
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}