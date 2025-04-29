# SmartBook

## Min biblioteksapp - en Övningsuppgift för Lexicon.

Programmet startar med att försöka läsa in böcker och lånekort från separata filer.
En välkomstsida visar status efter denna inläsning.
Om inga böcker kunde läsas in, så seedas systemet med ett antal titlar.
Därefter visas huvudmenyn.

<table>
<tr><td>======================================</td></tr>
<tr><td>MAIN MENU</td></tr>
<tr><td>======================================</td></tr>
<tr><td>[A] Add a book</td></tr>
<tr><td>[D] Delete a book</td></tr>
<tr><td>[L] Show all books</td></tr>
<tr><td>[F] Find a book</td></tr>
<tr><td>[C] Issue a new library card</td></tr>
<tr><td>[M] Borrow / return books</td></tr>
<tr><td>[W] Show borrowed books</td></tr>
<tr><td>[P] export books on loan to a file</td></tr>
<tr><td>[S] Save all books</td></tr>
<tr><td>[X] Exit</td></tr>
</table>

Menyn kan manövreras antingen med piltangenterna eller via snabbtangenter [?].
Aktuell rad visas i gult.
Enter eller snabbtangent för att aktivera ett menyalternativ.

Menyval M frågar efter kortnummer, 
Skapa ett sådant i menyalternativ C, 
och öppnar en undermeny:

<tr><td>======================================</td></tr>
<tr><td>LOGGED IN AS NNNNN</td></tr>
<tr><td>======================================</td></tr>
<tr><td>[B] Borrow books/td></tr>
<tr><td>[A] Return all books</td></tr>
<tr><td>[S] Select books to return</td></tr>
<tr><td>[X] Exit</td></tr>
</table>

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









