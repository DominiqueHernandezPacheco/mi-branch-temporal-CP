using Microsoft.Extensions.Configuration;

namespace Application.Core.Infraestructure.Services;

public class FileStorageService : IFileStorageService
{
    
    /// <summary>
    /// Es la ruta principal que se usa para el almacenamiento de archivos.
    /// </summary>
    private readonly string _rutaAlmacenamiento;
    
    /// <summary>
    /// Ruta para enviar un PDF genérico en caso de que ocurra un error al intentar leer un archivo.
    /// </summary>
    /// <c> Esto es útil en desarrollo, ya que en la base de datos se almanecenan los registros de las rutas de los archivos
    /// PDF, sin importar si estos se encuentran en el servidor de desarrollo, ocasionando que al intentar leer uno de esos
    /// archivos ocurra una excepción. No tiene utilidad en producción. Probablemente se elimine en un futuro. </c>
    private readonly string _rutaPdfError;


    public FileStorageService(string rutaAlmacenamiento, string rutaPdfError)
    {
        _rutaAlmacenamiento = rutaAlmacenamiento ?? throw new ArgumentNullException(nameof(rutaAlmacenamiento));
        _rutaPdfError = rutaPdfError;
    }

    /// <summary>
    /// Convierte una cadena en base64 a un archivo en el sistema de archivos
    /// </summary>
    /// <param name="base64Content">Cadena en Base 64 </param>
    /// <param name="filePath">Ruta donde se almacenará el archivo</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> ConvertBase64ToFileAsync(string base64Content, string filePath)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64Content);
            await File.WriteAllBytesAsync(filePath, bytes);
            return filePath;
        }
        catch (Exception ex)
        {
            throw new Exception($"Hubo un error al intentar crear el archivo. Detalles: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Convierte un archivo en el sistema de archivos a una cadena en base64
    /// </summary>
    /// <param name="filePath">Ruta del archivo</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> ConvertFileToBase64Async(string filePath)
    {
        try
        {
            byte[] bytes = await ReadFileAsync(filePath);
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            try
            {
                // Intenta leer el archivo genérico de error si la lectura del archivo original falla.
                byte[] errorBytes = await ReadFileAsync(_rutaPdfError);
                return Convert.ToBase64String(errorBytes);
            }
            catch
            {
                throw new Exception($"Hubo un error al intentar leer el archivo y el archivo de error. Detalles del error original: {ex.Message}");
            }
        }
    }

    private async Task<byte[]> ReadFileAsync(string path)
    {
        if (File.Exists(path))
        {
            return await File.ReadAllBytesAsync(path);
        }
        throw new FileNotFoundException($"El archivo en {path} no se encuentra.");
    }
    

    public string GenerateUniqueFilePath(string path2)
    {
        string uniqueFileName = Guid.NewGuid().ToString("N");
        var rutaArchivo = Path.Combine(_rutaAlmacenamiento, path2, uniqueFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(rutaArchivo)!);

        return rutaArchivo;
    }

    /// <summary>
    /// Elimina un archivo con la ruta especificada
    /// </summary>
    /// <param name="filePath"></param>
    /// <exception cref="Exception"></exception>
    public bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Hubo un error al intentar eliminar el archivo. Detalles: {ex.Message}");
        }
    }
}