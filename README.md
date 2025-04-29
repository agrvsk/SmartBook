# SmartBook

## Min biblioteksapp - en Övningsuppgift för Lexicon.

Programmet startar med att försöka läsa in böcker och lånekort från separata filer.
En välkomstsida visar status efter denna inläsning.
Om inga böcker kunde läsas in, så seedas systemet med ett antal titlar.
Därefter visas huvudmenyn.

<table>
<tr>======================================</tr>
<tr>MAIN MENU</tr>
<tr>======================================</tr>

</table>

[A] Add a book
[D] Delete a book
[L] Show all books
[F] Find a book
[C] Issue a new library card
[M] Borrow / return books
[W] Show books on loan
[P] export books on loan to a file
[S] Save all books
[X] Exit

Menyn kan manövreras antingen med piltangenterna eller via snabbtangenter [?].
Aktuell rad visas i gult.
Enter eller snabbtangent för att aktivera ett menyalternativ.

Menyval M frågar efter kortnummer, 
Skapa ett sådant i menyalternativ C, 
och öppnar en undermeny:
======================================
LOGGED IN AS NNNNN
======================================
[B] Borrow books
[A] Return all books
[S] Select books to return
[X] Exit

Filer:
<table>
<th>
	<td>Typ</td>
	<td>Fil</td>
	<td>Info</td>
</th>

<tr>
	<td>Class</td>
	<td>Program.cs</td>
	<td>Main för programstart</td>
</tr>
<tr>
	<td>Class</td>
	<td>InputControl.cs</td>
	<td>Basic hantering av inmatning</td>
</tr>
<tr>
	<td>Class</td>
	<td>LibraryApp.cs</td>
	<td>Menyhantering</td>
</tr>
<tr span=3>
Console metoder och File/IO finns endast i ovanstående filer. 
</tr>
<tr>
	<td>Class</td>
	<td>Library.cs</td>
	<td>Hanterar böcker m.h.a en List<Book></td>
</tr>
<tr>
	<td>Class</td>
	<td>LibraryCardHandler.cs</td>
	<td>Hanterar lånekort m.h.a. en List<Card></td>
</tr>
<tr>
	<td>Record</td>
	<td>Book.cs</td>
	<td>Genereras om med alternativ konstruktor vid utlåning/inlåning.</td>
</tr>
<tr>
	<td>Record</td>
	<td>Card.cs</td>
	<td>Lånekort ändras inte.</td>
</tr>
<tr>
	<td>Enum</td>
	<td>Category.cs</td>
	<td>Bok genre</td>
</tr>

</table>









