
<h2 align="center">
  <a href="https://www.moniqueaxt.com/portfolio/csharp-project/" target="_blank">
    Project webpage
  </a>
</h2>

<h4 align="center">A Blackjack game written in C# .NET</h4>
<p align="center"> Play as a human yourself, or set up a game of bots and see who wins. Created to play around with this specific programming language, learn about turn-based game logic and work with a database.</p>

<img src="https://www.moniqueaxt.com/images/gallery-BJ-Main_screen_1.jpg" width="24%"></img>
<img src="https://www.moniqueaxt.com/images/gallery-BJ-Main_screen_2.jpg" width="24%"></img>
<img src="https://www.moniqueaxt.com/images/gallery-BJ-Game_settings.jpg" width="24%"></img>
<img src="https://www.moniqueaxt.com/images/gallery-BJ-database.jpg" width="24%"></img>

## Design

The WPF application is designed with separation of logic and presentation in mind, via implementation of a three-layer architecture: presentation layer, business logic layer, and data access layer. Delegates are used together with events to create an event-driven application.

## Game

In addition to making the application modular and maintainable, the separation of layers was important to allow interchangeability of the GUI for the game in the future. Inspiration for handling the running of the game was taken from [roguelike games](https://en.wikipedia.org/wiki/Roguelike), using a turn-based game loop.

## Data logging

Game events are logged to a text file and to output. (Previous version: serialization was used to persist data in binary and XML formats, enabling the loading and saving of games from file).

## Database

A database manager class using LINQ in conjuction with a DbContext class implements SQL database functionality to allow the saving, deleting, editing and searching of games stored locally. 

## Misc

A simple timer and animation was used to shift colours in the banner of the main game window.

## Further development

- loading of games from the database
- saving mid-game (involves storing moves yet to be made but already queued)
- implementation of betting on each round (the actual fun part of playing)
