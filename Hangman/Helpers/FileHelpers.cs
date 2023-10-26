using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hangman.Helpers
{
    public static class FileHelpers
    {

        private static string _filePathDictionary = Path.Combine(Environment.CurrentDirectory, "HangmanWordsAndExplanations.txt");
        private static string _slownikSrc = "ANTROPOW » Aleksiej, 1716-95, malarz rosyjski, SEMITA » potomek jednego z synów Noego, URIELECZEK » zdrobnienie od męskiego imienia Uriel, EFIALTES » imię męskie, ASANECZEK » zdrobnienie od męskiego imienia Asan, DONATIENECZEK » zdrobnienie od męskiego imienia Donatien, CENTURY » auris, ASTORIA » klub sportowy z Bydgoszczy, NIEMIRECZEK » zdrobnienie od męskiego imienia Niemir, SPARTAKIADA » olimpiada raczej młodzieżowa, GORAZDZIK » zdrobnienie od męskiego imienia Gorazd, JARKA » zdrobnienie od żeńskiego imienia Jarosława, SPASSKI » Borys, mistrz szachowy, NIEMO » nie mówiąc, nie odzywając się, OLEANDROWY » skadający się z oleandrów, GRAWERUNEK » ryt, BEDNARSKA » Maria, 1903-79, polska aktorka, CZYTELNICZKA » zamówiła prenumeratę, ERSKINE » szkockie imię pochodzenia celtyckiego - ze szczytu klifu, mieszkaniec szczytu klifu, HIERRO » Ferro, wyspa hiszpańska w archipelagu Wysp Kanaryjskich, DYWIZ » łącznik graficzny, MACHINIMA » technika animacji polegająca na wykorzystaniu animacji z gier, ASAD » baszaszar Al-..., syryjski dyktator, PLACE » Zgody i Pigalle w Paryżu, LICYNIUSZECZEK » zdrobnienie od męskiego imienia Licyniusz, SMRODLIWIE » cuchnąco, śmierdząco, KALISTRAT » imię męskie, GRUPOWA » odpowiada za zespół ludzi, JULIUSZ » imię pochodzenia łacińskiego, zdrobnienia: Juliuszek, Julek, HERMISIA » zdrobnienie od żeńskiego imienia Hermina, KOKOSY » intratne, korzystne interesy, KWADROFONIA » przestrzenny dźwięk, EGMONT » tragedia Goethego, WYRACHOWYWANIE » kalkulacja, DADZBOGUSIA » zdrobnienie od żeńskiego imienia Dadzboga, MASON » trzymał fason w loży, PIRS » pomost przeładunkowy w porcie, WILCZYCA » mleczna matka Romulusa i Remusa, NAROL » gmina w powiecie lubaczowskim, STRACHOMIR » imię męskie, PLATONIDECZKA » zdrobnienie od żeńskiego imienia Platonida, DYRYGOWANIE » cheironomia, SZPICA » pododdział ubezpieczający wojsko, MOL » Alojzy, montażysta nie tylko ”Reksia”, HEKSAN » węglowodór z ropy naftowej, CHABAROWSZCZANKA » żyje w Chabarowsku, RONNE » główne miasto wyspy Bornholm, FRANCO » Francisco, dyktator, STYLIK » zdrobnienie od styl, JUDA » biblijny syn Jakuba i Lei, DYSGRAFIA » częściowa lub całkowita utrata umiejętności pisania, TYBURCJECZKA » zdrobnienie od żeńskiego imienia Tyburcja, COLUMBA » inaczej Gołąb, MAKSUSIA » zdrobnienie od żeńskiego imienia Maksymiliana, GLINOJECCZANKA » pani z Glinojecka, PRAJANKA » mieszka w Prai, OPOKI » niewzruszone podstawy, fundamenty, DOMICJANA » imię kobiece, DRUSZKIEWICZ » Stanisław, 1621-97, pamiętnikarz, żołnierz, kasztelan, MASNY » Tomasz, zbojnik beskidzki grasujący w końcu XVII w na terenie Żywiecczyzny, OPYCHANIE » pozbywanieć się, wyzbywanieć się, PERKUN » perun, GIULIANO » Amato, włoski polityk, były premier, ZATRZYMYWANIE » ryglowanie, HIPERMETROPIA » wada refrakcji oka, FOTOSTOP » planowe zatrzymanie pociągu specjalnego w celu wykonania zdjęć, GALLOPER » model auta marki Hyundai, KRYTYKA » krytyczna ocena filmu lub sztuki, LIKIEREK » słodki trunek, COBO » Yohana, hiszpańska aktorka, ODOLANECZEK » zdrobnienie od męskiego imienia Odolan, KAZIMIERA » imię kobiece, IJOLA » dramat Żuławskiego, BILER » taksówkarz z komedii Machulskiego, PYRKOSZ » grał w ”Janosiku”, CHUDZISZEWSKA » Kasia ..., postać grana przez Paulinę Chapko w ”Ludziach Chudego”, SKRYTKI » członkinie bezhabitowych zgromadzeń zakonnych, KEIRA » Knightley, aktorka, zagrała Annę Kareninę, OBCESOWY » ordynarny, wulgarny, NIETUZINKOWY » dziwny, unikatowy, unikalny, POPULARYZATORSKI » przymiotnik od przysłówka popularyzatorsko, KIBICE » dopingują graczy, KILOGRAMOMETR » jednostka pracy zużytej na podniesienie jednego kilograma do wysokości jednego metra, BOLA » broń Indian Ameryki Południowej, ELECTRA » model auta marki Buick, BRAJL » alfabet dla niewidomych, ALBERTA » imię kobiece, DIRAC » angielski fizyk, Paul ..., TUWALCZYCY » mieszkają w Tuvalu, CZERKIES » mieszka w Karaczajo-Czerkiesji, KANONY » normy i wzorce postępowania, SZCZEPKOWSKI » Zbigniew ..., polski kolarz, NIEWIDZIOCHA » niewarta siostra Agatki, REJESTROWY » dotyczący rejestru lub związany z rejestrem, GRZMOT » akustyczny efekt uderzania pioruna, INKRUTOWINY » dawniej parapetówa, RADOMIRA » imię żeńskie, SKOWYT » skarga kundla, GNEJS » skała metamorficzna używana w budownictwie, NIKODEM » imię męskie, ";
        private const char Separator = '»';

        public static bool FileExists()
        {
            return File.Exists(_filePathDictionary);
        }

        public static void CreateNewFile()
        {
            if (!File.Exists(_filePathDictionary))
                File.AppendAllText(_filePathDictionary, _slownikSrc);
        }

        static public Dictionary<string, string> GetAwailableWordsWithExplanation()
        {
            var dictionary = new Dictionary<string, string>();
            var words = DeserializeSlownikToList();

            if (words.Count == 0)
                throw new Exception("Nie mam żadnych słów do losowania, uzupełnij plik!");

            foreach (var line in words)
            {
                try
                {
                    var wordAndExplanation = line.Split(Separator);
                    dictionary.Add(wordAndExplanation[0].ToUpper().Trim(), wordAndExplanation[1].Trim());
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

            return dictionary;
        }

        static public List<string> DeserializeSlownikToList()
        {
            string slownik = ReadDictionaryFromFile();
            List<string> slownikLst = slownik.Trim().Split(' ').Select(x => x.Trim()).ToList();
            List<string> slownikLstOut = new List<string>();
            StringBuilder sb = new StringBuilder();
            int x = 0;

            for (int i = 0; i < slownikLst.Count; i++)
            {
                try
                {
                    sb.Append(slownikLst[i] + " ");

                    if (i > 1
                        && slownikLst[i] != Separator.ToString()
                        && !int.TryParse(slownikLst[i].Substring(0, 1), out x)
                        && slownikLst[i] != "...,"
                        && !slownikLst[i].Contains("-")
                        && slownikLst[i].Trim().ToUpper() == slownikLst[i].Trim())
                    {
                        if (i < slownikLst.Count)
                            slownikLstOut.Add(sb.ToString().Trim().Replace(slownikLst[i], ""));
                        sb.Clear();
                        sb.Append(slownikLst[i] + " ");
                    }

                    if (i == slownikLst.Count - 1) slownikLstOut.Add(sb.ToString().Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            return slownikLstOut;
        }

        private static string ReadDictionaryFromFile()
        {
            return File.ReadAllText(_filePathDictionary);
        }
    }
}
