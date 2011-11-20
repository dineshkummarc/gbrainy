<%@ Page Language="C#" Inherits="gbrainy.Clients.WebForms.Help" MasterPageFile="~/MasterPage.master" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>
<asp:Content ContentPlaceHolderID="main_placeholder" ID="main_placeholderContent" runat="server">
	
	<h1>Introduction</h1>
	<p>
		<app>gbrainy</app> is a brain teaser game; the aim of the game is to have fun and keep 
		your brain trained. 
	</p>
	<p>
		It features different game types like logic puzzles, mental calculation games, 
		memory trainers and verbal analogies, designed to test different cognitive skills.
	</p>
	<p>
		<app>gbrainy</app> is enjoyable for kids, adults or senior citizens
	</p>
	<p>
		<app>gbrainy</app> relies heavily on the work of previous puzzle masters, 
		ranging from classic puzzles from ancient times to more recent works like 
		<link href="http://en.wikipedia.org/wiki/Terry_Stickels">Terry Stickels'</link> 
		puzzles or the classic <link href="http://en.wikipedia.org/wiki/Dr._Brain">Dr. Brain</link> 
		game.
	</p>
	<p>
		There have been recent discussions in the scientific community regarding whether 
		brain training software improves cognitive performance. Most of the 
		studies show that there is little or no improvement, but that doesn't mean 
		you can't have a good time playing games like <app>gbrainy</app>!
	</p>
	<h1>Game types</h1>
	<!-- Logic -->
	<span class="WelcomeLeft">
	 	<img src = "images/logic-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Games designed to challenge your reasoning and thinking skills. These 
            	games are based on sequences of elements, visual and spatial reasoning 
           	or relationships between elements.
	</span>
	<br/>
	<br/>

	<!-- Calculation -->
	<span class="WelcomeLeft">
	 	<img src = "images/math-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Games based on arithmetical operations designed to improve your mental
            	calculation skills. Games that require the player to use multiplication,
            	division, addition and subtraction combined in different ways.
	</span>
	<br/>
	<br/>

	<!-- Verbal -->
	<span class="WelcomeLeft">
	 	<img src = "images/verbal-games-32.png"/>
	</span>

	<span class="WelcomeRight">
		Games that challenge your verbal aptitude. These games ask the player to identify cause and effect, use synonyms or antonyms, and use their vocabulary.
	</span>	
	
	<h1>Tips</h1>
	Some tips that may be useful when playing gbrainy:
	<ul><li>Read the instructions carefully and identify the data and given clues.</li></ul>
	<ul><li>To score the player gbrainy uses the time and tips needed to complete each game.</li></ul>
	<ul><li>In logic games, elements that may seem irrelevant can be very important.</li></ul>
	<ul><li>Try to approach a problem from different angles.</li></ul>
	<ul><li>Do not be afraid of making mistakes, they are part of the learning process.</li></ul>
	<ul><li>Do all the problems, even the difficult ones. Improvement comes from challeging yourself.</li></ul>
	<ul><li>Play on a daily basis, you will notice progress soon.</li></ul>
	<ul><li>Association of elements is a common technique for remembering things.</li></ul>
	<ul><li>Grouping elements into categories is a common technique for remembering things.</li></ul>
	<ul><li>Build acronyms using the first letter of each fact to be remembered.</li></ul>
	<ul><li>The enjoyment obtained from a puzzle is proportional to the time spent on it.</li></ul>
	<ul><li>Think of breaking down every problem into simpler components.</li></ul>
	<ul><li>When answering verbal analogies pay attention to the verb tense.</li></ul>
</asp:Content>
