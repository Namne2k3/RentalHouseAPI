﻿using RentalHouse.Domain.Entities.Favorites;

namespace RentalHouse.Application.DTOs.Conversions
{
    public static class FavoriteConversion
    {
        public static Favorite ToFavorite(this FavoriteDTO favoriteDTO)
        {
            return new Favorite
            {
                NhaTroId = favoriteDTO.NhaTroId
            };
        }
        public static FavoriteDTO ToFavoriteDTO(this Favorite favorite)
        {
            return new FavoriteDTO(favorite.NhaTroId);
        }

        public static IEnumerable<int> ToListSavedRentalId(IEnumerable<Favorite> favorites)
        {
            return favorites.Select(f => f.NhaTroId).ToList();
        }
    }
}
