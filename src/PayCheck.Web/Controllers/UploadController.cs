namespace PayCheck.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;

    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly string _folderName = "Arquivos";

        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> SendFile(List<IFormFile> files)
        {
            var returnView = View("Publicacao");

            var filesLength = files.Sum(f => f.Length);

            var tempFileName = Path.GetTempFileName();

            //  Processa os arquivos enviados.
            foreach (var file in files)
            {
                //  Verifica se existe um arquivo (ou mais) e se não está vazio.
                if (file == null ||
                    file.Length == 0)
                {
                    //retorna a viewdata com erro
                    returnView.ViewData["Erro"] = "Erro: Arquivo(s) não selecionado(s)";

                    return returnView;
                }

                // < define a pasta onde vamos salvar os arquivos >
                string subFolderName = @"Publicacoes\Recebidos";

                // Define um Guid para o arquivo enviado.
                var guidPublicacao = Guid.NewGuid().ToString("N").ToUpper();

                string fileName = $@"{guidPublicacao}";

                //verifica qual o tipo de arquivo : jpg, gif, png, pdf ou tmp
                if (file.FileName.Contains(".jpg"))
                    fileName = $@"{fileName}.jpg";

                else if (file.FileName.Contains(".gif"))
                    fileName = $@"{fileName}.gif";

                else if (file.FileName.Contains(".png"))
                    fileName = $@"{fileName}.png";

                else if (file.FileName.Contains(".pdf"))
                    fileName = $@"{fileName}.pdf";

                else if (file.FileName.Contains(".rtf"))
                    fileName = $@"{fileName}.rtf";

                else
                    fileName = $@"{fileName}.tmp";

                //< obtém o caminho físico da pasta wwwroot >
                string webRootPath = this._webHostEnvironment.WebRootPath;

                // monta o caminho onde vamos salvar o arquivo : 
                // ~\wwwroot\Arquivos\Arquivos_Usuario\Recebidos
                string pathDestFileName = $@"{webRootPath}\{this._folderName}\{subFolderName}\{fileName}";

                // incluir a pasta Recebidos e o nome do arquivo enviado : 
                // ~\wwwroot\Arquivos\Arquivos_Usuario\Recebidos\
                //string caminhoDestinoArquivoOriginal = caminhoDestinoArquivo + "\\Recebidos\\" + fileName;
                //copia o arquivo para o local de destino original
                using (var fileStream = new FileStream(
                    pathDestFileName,
                    FileMode.Create))
                {
                    await file.CopyToAsync(
                        fileStream);
                }
            }

            returnView.ViewData["Resultado"] = $"{files.Count} arquivo(s) foi(ram) enviado(s) ao servidor com tamanho total de {filesLength} bytes.";

            //monta a ViewData que será exibida na view como resultado do envio 

            return returnView;
        }
    }
}