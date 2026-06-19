//  IPersistivel.cs — Interface (ABSTRAÇÃO)

namespace SavePoint.Models
{
    public interface IPersistivel
    {
        void Salvar(string caminho);
        void Carregar(string caminho);
    }
}
