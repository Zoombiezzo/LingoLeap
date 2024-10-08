using System;
using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.RequirementService
{
    public class RequirementService : IRequirementService
    {
        private readonly Dictionary<IRequirable, List<IRequirement>> _itemRequirements;
        private readonly Dictionary<Type, List<IRequirable>> _requirementsItems;

        private readonly Dictionary<Type, IRequirementSystem> _requirementSystems;
        private readonly List<IRequirementSystem> _systems;

        public RequirementService()
        {
            _itemRequirements = new Dictionary<IRequirable, List<IRequirement>>(16);
            _requirementsItems = new Dictionary<Type, List<IRequirable>>(16);
            _requirementSystems = new Dictionary<Type, IRequirementSystem>(8);
            _systems = new List<IRequirementSystem>(8);
        }

        public void Register(IRequirable item, IReadOnlyList<IRequirement> requirements)
        {
            if (_itemRequirements.TryGetValue(item, out var itemRequirements) == false)
            {
                itemRequirements = new List<IRequirement>();
                _itemRequirements.Add(item, itemRequirements);
            }

            foreach (var requirement in requirements)
            {
                itemRequirements.Add(requirement);
         
                var type = requirement.GetType();

                if (_requirementsItems.TryGetValue(type, out var list) == false)
                {
                    list = new List<IRequirable>(4);
                    _requirementsItems.Add(type, list);
                }

                list.Add(item);
            }

            if (CheckAll(item))
                item.RequirementEnable();
            else
                item.RequirementDisable();
        }

        public void Register(IRequirable item, IRequirement requirement)
        {
            if (_itemRequirements.TryGetValue(item, out var itemRequirements) == false)
            {
                itemRequirements = new List<IRequirement>();
                _itemRequirements.Add(item, itemRequirements);
            }
            
            itemRequirements.Add(requirement);
            
            var type = requirement.GetType();

            if (_requirementsItems.TryGetValue(type, out var list) == false)
            {
                list = new List<IRequirable>(4);
                _requirementsItems.Add(type, list);
            }

            list.Add(item);
            
            if (CheckAll(item))
                item.RequirementEnable();
            else
                item.RequirementDisable();
        }

        public void Unregister(IRequirable item)
        {
            if (_itemRequirements.TryGetValue(item, out var itemRequirement) == false)
                return;
            
            foreach (var requirement in itemRequirement)
            {
                var type = requirement.GetType();
                
                if(_requirementsItems.TryGetValue(type, out var requirements) == false)
                    continue;
                
                if (requirements.Contains(item) == false)
                    continue;
                
                requirements.Remove(item);
            }
            
            _itemRequirements.Remove(item);
        }

        public bool Check(IRequirement requirement)
        {
            var type = requirement.GetType();
            
            if (_requirementSystems.TryGetValue(type, out var requirementSystem) == false)
                return false;
            
            return requirementSystem.Check(requirement);
        }

        public bool CheckAll(IRequirable requirable)
        {
            if (_itemRequirements.TryGetValue(requirable, out var requirementsItem) == false)
                return false;

            foreach (var requirement in requirementsItem)
            {
                if (Check(requirement) == false)
                    return false;
            }

            return true;
        }
        
        public void RegisterRequirementSystem<T>(IRequirementSystem requirementSystem) where T : IRequirement
        {
            var type = typeof(T);
            
            if (_requirementSystems.TryAdd(type, requirementSystem) == false)
            {
                Debugger.Log($"[RequirementService]: Requirement system {type} already exists");
                return;
            }
            
            _systems.Add(requirementSystem);
            
            CheckRequirementsByType<T>();
        }

        public void EnableSystems()
        {
            foreach (var system in _systems)
            {
                system.Enable();
            }
        }

        public void DisableSystems()
        {
            foreach (var system in _systems)
            {
                system.Disable();
            }
        }

        public void CheckRequirementsByType<T>() where T : IRequirement
        {
            var type = typeof(T);
            
            if(_requirementsItems.TryGetValue(type, out var requirementsItem) == false)
                return;

            foreach (var item in requirementsItem)
            {
                if (item.RequirementEnabled) continue;

                if (CheckAll(item))
                {
                    item.RequirementEnable();
                }
            }
        }
    }
}