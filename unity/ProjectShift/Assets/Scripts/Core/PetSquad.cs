using System;
using System.Collections.Generic;

namespace ProjectShift.Core
{
    /// <summary>Home posts per spec §15 (home guardians).</summary>
    public enum HomePost
    {
        None,
        PerimeterPatrol,
        GateGuardian,
        Scout,
        Defender,
        ResourceGuardian,
        SupportCompanion,
        Resting
    }

    [Serializable]
    public sealed class PetEntryData
    {
        public string PetId;
        public string SpeciesId;
        public bool InJourneySquad;
        public HomePost HomePost = HomePost.Resting;
    }

    [Serializable]
    public sealed class PetSquadData
    {
        public List<PetEntryData> Pets = new List<PetEntryData>();
    }

    /// <summary>
    /// Journey-squad + home-post rules (PET-005). Max 5 active; moving a pet
    /// to the squad parks its home post and restores it on return.
    /// </summary>
    public sealed class PetSquad
    {
        public const int MaxJourneySquad = 5;

        private readonly Dictionary<string, PetEntryData> _pets =
            new Dictionary<string, PetEntryData>(StringComparer.Ordinal);

        public int JourneyCount
        {
            get
            {
                int n = 0;
                foreach (var p in _pets.Values) if (p.InJourneySquad) n++;
                return n;
            }
        }

        public Result RegisterPet(string petId, string speciesId, HomePost homePost = HomePost.Resting)
        {
            Guard.NotNullOrEmpty(petId, nameof(petId));
            Guard.NotNullOrEmpty(speciesId, nameof(speciesId));
            if (_pets.ContainsKey(petId)) return Result.Fail($"Duplicate pet '{petId}'.");
            _pets.Add(petId, new PetEntryData
            {
                PetId = petId,
                SpeciesId = speciesId,
                InJourneySquad = false,
                HomePost = homePost
            });
            return Result.Success();
        }

        public Result AssignToSquad(string petId)
        {
            if (!_pets.TryGetValue(petId, out var p))
                return Result.Fail($"Unknown pet '{petId}'.");
            if (p.InJourneySquad) return Result.Fail("Pet already in squad.");
            if (JourneyCount >= MaxJourneySquad)
                return Result.Fail($"Squad is full ({MaxJourneySquad}).");
            p.InJourneySquad = true; // home post preserved for restore
            return Result.Success();
        }

        public Result ReturnHome(string petId)
        {
            if (!_pets.TryGetValue(petId, out var p))
                return Result.Fail($"Unknown pet '{petId}'.");
            if (!p.InJourneySquad) return Result.Fail("Pet is not in squad.");
            p.InJourneySquad = false; // previous home post restored implicitly
            return Result.Success();
        }

        public Result SetHomePost(string petId, HomePost post)
        {
            if (!_pets.TryGetValue(petId, out var p))
                return Result.Fail($"Unknown pet '{petId}'.");
            p.HomePost = post;
            return Result.Success();
        }

        public bool TryGet(string petId, out PetEntryData pet) => _pets.TryGetValue(petId, out pet);

        public PetSquadData ToData()
        {
            var data = new PetSquadData();
            foreach (var p in _pets.Values)
                data.Pets.Add(new PetEntryData
                {
                    PetId = p.PetId,
                    SpeciesId = p.SpeciesId,
                    InJourneySquad = p.InJourneySquad,
                    HomePost = p.HomePost
                });
            return data;
        }

        public void LoadData(PetSquadData data)
        {
            Guard.NotNull(data, nameof(data));
            _pets.Clear();
            foreach (var p in data.Pets)
            {
                if (p == null || string.IsNullOrEmpty(p.PetId))
                    throw new ArgumentException("Corrupt pet entry.");
                if (_pets.ContainsKey(p.PetId))
                    throw new ArgumentException($"Duplicate pet '{p.PetId}' in save.");
                _pets.Add(p.PetId, p);
            }
            if (JourneyCount > MaxJourneySquad)
                throw new ArgumentException("Save has oversized journey squad.");
        }
    }
}
