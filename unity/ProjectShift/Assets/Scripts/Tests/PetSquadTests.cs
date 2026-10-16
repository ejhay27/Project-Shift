using NUnit.Framework;
using ProjectShift.Core;

namespace ProjectShift.Tests
{
    /// <summary>EditMode tests for PET-005.</summary>
    public sealed class PetSquadTests
    {
        [Test]
        public void Squad_CappedAtFive()
        {
            var squad = new PetSquad();
            for (int i = 0; i < 6; i++)
                squad.RegisterPet("pet" + i, "species", HomePost.Resting);
            for (int i = 0; i < 5; i++)
                Assert.IsTrue(squad.AssignToSquad("pet" + i).Ok);
            Assert.IsFalse(squad.AssignToSquad("pet5").Ok);
            Assert.AreEqual(5, squad.JourneyCount);
        }

        [Test]
        public void ReturnHome_RestoresPreviousPost()
        {
            var squad = new PetSquad();
            squad.RegisterPet("p1", "wolf", HomePost.GateGuardian);
            squad.AssignToSquad("p1");
            squad.ReturnHome("p1");
            Assert.IsTrue(squad.TryGet("p1", out var pet));
            Assert.IsFalse(pet.InJourneySquad);
            Assert.AreEqual(HomePost.GateGuardian, pet.HomePost);
        }

        [Test]
        public void DuplicatePet_Rejected()
        {
            var squad = new PetSquad();
            Assert.IsTrue(squad.RegisterPet("p1", "wolf").Ok);
            Assert.IsFalse(squad.RegisterPet("p1", "wolf").Ok);
        }
    }
}
