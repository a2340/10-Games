# Space Dodge (Unity, 3 Canlı)

Bu depo, HTML5/canvas sürümü verilen "Space Dodge" mini oyununu Unity için yeniden yazar. Oyuncu yatay eksende hareket eden küçük bir gemiyi kontrol eder; yukarıdan düşen asteroitlerden kaçmaya çalışırken skor kazanır, seviyeyi yükseltir ve 3 canla devam eder.

## Özellikler
- 3 can, vurulunca kısa süre yenilmezlik ve ekran flaşı.
- Skor, en iyi skor (PlayerPrefs), seviye artışı (her 10 puanda +1) ve gerçek zamanlı UI güncellemeleri.
- Zorluk, seviye ilerledikçe düşen asteroid sayısı ve hızı artarak yükselir.
- Basit yıldız alanı arka planı ve küçük gemi ikonlarıyla can göstergesi.
- Klavye (A/D veya Oklar) ve dokunma kontrollü yatay hareket.

## Kurulum
1. Unity'de 2D bir proje açın ve bu depodaki `Assets` klasörünü köke kopyalayın.
2. Önerilen katmanlar/nesneler:
   - **Main Camera**: Ortografik, boyutu sahne yüksekliğine göre ayarlayın.
   - **GameController** (empty GameObject): `GameController` scriptini ekleyin.
   - **UI**: Canvas (Screen Space - Overlay) altında TextMeshPro (veya UI Text) bileşenleri için skor, en iyi skor, seviye, can ve bilgi yazıları; yeniden başlatma butonu; seviye flaşı ve game over paneli için GameObjects.
   - **Player**: Basit bir sprite (üçgen/ok) + `PlayerShip` scripti + `BoxCollider2D` (Is Trigger) + `SpriteRenderer`. Tag: `Player`.
   - **Asteroid Prefab**: Dairesel sprite + `Asteroid` scripti + `CircleCollider2D` (Is Trigger) + `SpriteRenderer`.
   - **Star Field**: Birkaç küçük sprite nesnesi (veya `StarField` scriptini kullanan boş nesne) arka planı için.
3. `GameController` üzerindeki referans alanlarını Inspector'da doldurun: Player, Asteroid Prefab, UI metinleri, buton ve paneller.
4. Oynatmaya basın. Space veya dokunarak oyunu başlatabilir, A/D ya da oklarla hareket edebilirsiniz.

## Proje dosyaları
- `Assets/Scripts/GameController.cs`: Oyun akışı, skor/seviye, can, spawn mantığı ve UI güncellemeleri.
- `Assets/Scripts/PlayerShip.cs`: Yatay hareket, dokunma/klavye girdisi, gemi sınırlandırması ve vurulma yenilmezliği.
- `Assets/Scripts/Asteroid.cs`: Asteroid hareketi, yan duvarlardan sekme, düşme sonrası skor bildirimi ve çarpışma bildirimleri.
- `Assets/Scripts/StarField.cs`: Küçük yıldız parçacıklarını aşağı doğru kaydırarak arka plan efekti sağlar.

> Not: Bu proje, Unity Physics yerine basit transform güncellemeleri ve trigger collider'lar kullanır. Gerekirse materyal/sprite düzenlemeleriyle görselliği güçlendirebilirsiniz.
