/*
    Install-Package MailKit
    Install-Package MimeKit
 */

using imap_samemu;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;


using System.Text.RegularExpressions;

class Program
{
    private static readonly string Credentials = "C:/Users/wojtek/Desktop/imap/pass.json";

    static async Task main()
    {
        Console.WriteLine("=== Gmail Reader ===/n");

        Console.WriteLine("Wczytywanie pliku JSON");

       Wczytywanie();


    }
    static async Task<Json> Wczytywanie()
    {


        return null;
    }
}

