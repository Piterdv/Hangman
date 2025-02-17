using Hangman.Models;
using System.Threading.Tasks;

namespace Hangman.SynchronizeSource;
public interface ISynchronizeResource
{
    Task<StateOfSynchronization> CheckChangesInSource(string dictionaryDirPath);
}