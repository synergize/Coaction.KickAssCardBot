# Coaction KickAssCardBot - Command Reference

## Card Data

All card data is acquired via [Scryfall's](https://scryfall.com/) API.

## Available Commands

### `/card-lookup <cardName>`

**Description:** Acquires information on the specified card, including sets, price, artwork, author, legality, and rules.

**Usage:**

```
/card-lookup Fireball
```

---

### `/event-locator <zipCode>`

**Description:** Looks up Magic: The Gathering events within 25 miles of the specified ZIP code. A menu list provides more details about each event.

**Usage:**

```
/event-locator 90210
```

---

### `/roll <diceRoll>`

**Description:** Simulates rolling a dice based on the provided format.

**Usage:**

```
/roll 1d6
```

**Format:**

- `NdX` where `N` is the number of dice and `X` is the sides per die.

---

### `/decklist-count-lookup <tournamentId>`

**Description:** Retrieves deck lists from a tournament on [Melee.gg](https://melee.gg/) and aggregates them by round.

**Usage:**

```
/decklist-count-lookup 12345
```

---

### `/top-16-lookup <tournamentId>`

**Description:** Retrieves the top 16 decks from a tournament on [Melee.gg](https://melee.gg/).

**Usage:**

```
/top-16-lookup 67890
```

---

### `[[card name]]`

**Description:** Cards can also be looked up inline using double square brackets. Multiple cards in a sentence will trigger a thread with details for each card.

**Example:**

```
I love playing with [[Black Lotus]] and [[Ancestral Recall]]!
```

This will generate a thread providing information about both cards.

